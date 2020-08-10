using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Revealyan.Sodist.Core.WebApp.Configurations
{
    public class ApplicationConfiguration
    {
        #region data
        /// <summary>
        /// Имя приложения
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Список сборок, где расплагаются контроллеры, модели, вью и т.п.
        /// </summary>
        public List<string> Assembly { get; set; } = new List<string>();
        /// <summary>
        /// Использовать контрооллеры без представлений
        /// </summary>
        public bool UseControllers { get; set; }
        /// <summary>
        /// Использовать шаблон model-view-controller
        /// </summary>
        public bool UseMvc { get; set; }
        #endregion

    }
}
