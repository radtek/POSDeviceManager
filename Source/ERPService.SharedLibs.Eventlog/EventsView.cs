using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Вспомогательный класс для детального просмотра событий
    /// </summary>
    public static class EventsView
    {
        /// <summary>
        /// Показать событие
        /// </summary>
        /// <param name="viewLink">Реализация связи между списокм событий и диалогом</param>
        public static void Show(IEventsViewLink viewLink)
        {
            FormEventDetails formEventDetails = new FormEventDetails();
            formEventDetails.Execute(viewLink);
        }
    }
}
