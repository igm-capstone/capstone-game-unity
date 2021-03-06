﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Wenzil.Console
{

    /// <summary>
    /// The interactive front-end of the console.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConsoleController))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ConsoleUI : MonoBehaviour, IScrollHandler
    {
        private const int BufferLimit = 10000;

        public event Action<bool> onToggleConsole;
        public event Action<string> onSubmitCommand;
        public event Action onClearConsole;

        public Scrollbar scrollbar;
        public Text outputText;
        public ScrollRect outputArea;
        public InputField inputField;
        private CanvasGroup canvasGroup;

        public bool IsActive
        {
            get { return canvasGroup.interactable && canvasGroup.blocksRaycasts; }
            set
            {
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
                canvasGroup.alpha = value ? 1 : 0;

                if (value)
                {
                    inputField.Select();
                    inputField.ActivateInputField();
                }
            }
        }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Show(false);
        }

        /// <summary>
        /// Opens or closes the console.
        /// </summary>
        public void ToggleConsole()
        {
            IsActive = !IsActive;
        }

        /// <summary>
        /// Opens the console.
        /// </summary>
        public void OpenConsole()
        {
            IsActive = true;
        }

        /// <summary>
        /// Closes the console.
        /// </summary>
        public void CloseConsole()
        {
            IsActive = false;
        }

        void OnEnable()
        {
            OnToggle(true);
        }

        void OnDisable()
        {
            OnToggle(false);
        }

        private void OnToggle(bool open)
        {
            Show(open);

            if (open)
                inputField.ActivateInputField();
            else
                ClearInput();

            if (onToggleConsole != null)
                onToggleConsole(open);
        }

        private void Show(bool show)
        {
            inputField.gameObject.SetActive(show);
            outputArea.gameObject.SetActive(show);
            scrollbar.gameObject.SetActive(show);
        }

        /// <summary>
        /// What to do when the user wants to submit a command.
        /// </summary>
        public void OnSubmit(string input)
        {
            if (EventSystem.current.alreadySelecting) // if user selected something else, don't treat as a submit
                return;

            if (input.Length > 0)
            {
                if (onSubmitCommand != null)
                    onSubmitCommand(input);
                scrollbar.value = 0;
                ClearInput();
            }

            inputField.ActivateInputField();
        }

        /// <summary>
        /// What to do when the user uses the scrollwheel while hovering the console input.
        /// </summary>
        public void OnScroll(PointerEventData eventData)
        {
            scrollbar.value += 0.08f * eventData.scrollDelta.y;
        }

        /// <summary>
        /// Displays the given message as a new entry in the console output.
        /// </summary>
        public void AddNewOutputLine(object line)
        {
            outputText.text += Environment.NewLine + line;

            var extrapolate = outputText.text.Length - BufferLimit;
            if (extrapolate > 0)
            {
                outputText.text = outputText.text.Substring(extrapolate, BufferLimit);
            }
        }

        /// <summary>
        /// Clears the console output.
        /// </summary>
        public void ClearOutput()
        {
            outputText.text = "";
            outputText.SetLayoutDirty();
            if(onClearConsole != null)
                onClearConsole();
        }

        /// <summary>
        /// Clears the console input.
        /// </summary>
        public void ClearInput()
        {
            SetInputText("");
        }

        /// <summary>
        /// Writes the given string into the console input, ready to be user submitted.
        /// </summary>
        public void SetInputText(string input) 
        {
            inputField.MoveTextStart(false);
            inputField.text = input;
            inputField.MoveTextEnd(false);
        }
    }
}