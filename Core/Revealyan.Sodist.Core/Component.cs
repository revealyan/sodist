using Revealyan.Sodist.Core.Attributes;
using Revealyan.Sodist.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Revealyan.Sodist.Core
{
    public abstract class Component : IComponent
    {
        #region props-info
        public string Name { get; }
        #endregion

        #region data
        [Parameter(Name = "StartDate")]
        protected internal DateTime _startDate;
        #endregion

        #region ctors
        public Component(string name, Dictionary<string, object?>? parameters = null, object[]? dependencies = null)
        {
            Name = name;
            ResolveInjections(dependencies ?? new object[0]);
            ResolveParameters(new Dictionary<string, object?>(parameters));
        }
        #endregion

        #region supports
        protected void ResolveInjections(object[] dependencies)
        {
            var deps = dependencies.ToDictionary(d => d, d =>
            {
                var type = d.GetType();
                var res = new List<Type>(type.GetInterfaces());
                Type? baseType = type;
                while (baseType != null)
                {
                    res.Add(baseType);
                    baseType = baseType.BaseType;
                }
                return res;
            });

            var everythingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var injects = new List<FieldInfo>();
            var baseType = GetType();
            while(baseType != null)
            {
                var candidates = baseType.GetFields(everythingFlag).Where(inject => inject.GetCustomAttribute<InjectAttribute>() is InjectAttribute &&
                                                                                    !injects.Any(inj => inj.Name == inject.Name && inj.DeclaringType == inject.DeclaringType));
                injects.AddRange(candidates);
                baseType = baseType.BaseType;
            }

            foreach (var prop in injects)
            {
                var candidates = deps.Where(d => d.Value.Contains(prop.FieldType));
                if (candidates.Count() == 0)
                {
                    throw new ComponentException(this, $"Реализация для компонента \"{prop.FieldType.FullName}\" не найдена");
                }
                else if (candidates.Count() > 1)
                {
                    throw new ComponentException(this, $"Найдено несколько реализаций [{candidates.Aggregate(string.Empty, (r, c) => $"{r},\"{c.Key.GetType().FullName}\"", r => r.Trim(','))}] для компонента \"{prop.FieldType.FullName}\"");
                }
                prop.SetValue(this, candidates.First().Key);
            }
        }
        protected void ResolveParameters(Dictionary<string, object?> parameters)
        {
            var everythingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var @params = new List<(string Name, bool Required, FieldInfo Field)>();
            var baseType = GetType();
            while(baseType != null)
            {
                var candidates = baseType.GetFields(everythingFlag)
                                         .Where(param => param.GetCustomAttribute<ParameterAttribute>() is ParameterAttribute)
                                         .Select(param =>
                                         {
                                             var attr = param.GetCustomAttribute<ParameterAttribute>();
                                             var name = attr.Name;
                                             if (string.IsNullOrWhiteSpace(name))
                                             {
                                                 name = param.FieldType.Name;
                                             }
                                             (string Name, bool Required, FieldInfo Field) res = (name, attr.Required, param);
                                             return res;
                                         })
                                         .Where(param => !@params.Any(p => param.Field.Name == p.Field.Name && param.Field.DeclaringType == p.Field.DeclaringType));
                foreach (var candidate in candidates)
                {
                    if(@params.Any(p => p.Name == candidate.Name))
                    {
                        throw new ComponentException(this, $"Несколько параметров имеют одинаковое имя \"{candidate.Name}\"");
                    }
                }
                @params.AddRange(candidates);
                baseType = baseType.BaseType;
            }
            foreach (var param in @params)
            {
                if (parameters.ContainsKey(param.Name))
                {
                    try
                    {
                        param.Field.SetValue(this, parameters[param.Name]);
                    }
                    catch (Exception exc)
                    {
                        throw new ComponentException(this, $"Неудалось распарсить параметр \"{param.Name}\"", exc);
                    }
                }
                else if (param.Required)
                {
                    throw new ComponentException(this, $"Параметр \"{param.Name}\" явялется обязательным");
                }
            }

        }
        #endregion

        #region IComponent
        public virtual void Startup()
        {

        }

        public virtual void Shutdown()
        {

        }

        public virtual void Reboot()
        {
            try
            {
                Shutdown();
            }
            finally
            {
                Startup();
            }
        }
        #endregion
    }
}
