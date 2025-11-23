using System;
using System.Collections.Generic;

namespace DavPadDev.Services
{
    public class ModalService
    {
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string Note { get; private set; } = string.Empty;
        public List<ModalButton> Buttons { get; private set; } = new();

        public bool Show { get; private set; }

        public event Action? OnChange;

        public void ShowModal(string title, string message, string note = "", params ModalButton[] buttons)
        {
            Title = title;
            Message = message;
            Note = note;
            Buttons = new List<ModalButton>(buttons);
            Show = true;
            NotifyStateChanged();
        }

        public void CloseModal()
        {
            Show = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    public class ModalButton
    {
        public string Text { get; set; }
        public Action OnClick { get; set; }
        public string CssClass { get; set; } = "btn";

        public ModalButton(string text, Action onClick, string cssClass = "btn")
        {
            Text = text;
            OnClick = onClick;
            CssClass = cssClass;
        }
    }
}
