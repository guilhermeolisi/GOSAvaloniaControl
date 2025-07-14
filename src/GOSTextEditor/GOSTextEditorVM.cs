﻿using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using BaseLibrary;

namespace GOSAvaloniaControls;

public partial class GOSTextEditor : TemplatedControl
{
    TextDocument? Document { get; set; }
    TextEditorOptions? EditorOptions { get; set; }

    bool isSavingTextSync, changingFile;
    private IFileTXTIO fileManager;
    private Dispatcher UIDispatcher = Dispatcher.UIThread;

    public void IsClosing()
    {
        SaveCurrentDocumentText();
        fileManager.Closing();
    }
    private void SaveCurrentDocumentText()
    {
        if (!isTextChanged)
            return;

        isSavingTextSync = true;
        FileSave(GetDocumentText(), false);
    }
    string textTemp = string.Empty;
    Task? saveTask;
    bool isTextChanged = false;
    private async void FileSave(string text, bool isAsync)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
                return;

            if (textTemp is not null)
                textTemp = null;

            if (text is not null)
            {
                textTemp = text;
            }
            else
            {
                return;
            }

            if (saveTask is null || saveTask.IsCompleted)
            {
                string temp = null;
                while (isTextChanged)
                {
                    isTextChanged = false;
                    if (temp is not null && temp.CompareTo(textTemp) == 0)
                        return;
                    temp = textTemp.Substring(0);
                    saveTask = new Task(() => fileManager.WriteTXT(textTemp));
                    saveTask.Start();
                    if (isAsync)
                        await saveTask;
                    else
                        saveTask.Wait();

                }
            }
        }
        finally
        {
            if (isSavingTextSync)
                isSavingTextSync = false;
        }
    }
    public async Task SetNewFile()
    {
        if (Document is null)
            return;
        fileManager.SetPathFile(string.IsNullOrWhiteSpace(FilePath) ? null : FilePath.Substring(0));
        changingFile = true;
        if (!string.IsNullOrWhiteSpace(FilePath) && !Directory.Exists(Path.GetDirectoryName(FilePath)))
        {
            //TODO throw exception
            return;
        }
        if (!string.IsNullOrWhiteSpace(FilePath) && !await fileManager.ReadTXTAsync())
        {
            //TODO pegar o erro de acesso
            return;
        }

        string text;
        text = fileManager.TextResult;

        ReplaceDocument(text is not null ? text : string.Empty);

        UIDispatcher.Post(() =>
        {
            Document.UndoStack.ClearAll();
        }, DispatcherPriority.Send);
        changingFile = false;
    }
    private string GetDocumentText()
    {
        if (Document is null)
            return null;

        if (UIDispatcher.CheckAccess())
        {
            return Document.Text.Substring(0);
        }
        else
        {
            bool isGo = false;
            string result = null;
            UIDispatcher.Post(() =>
            {
                result = Document.Text.Substring(0);
                isGo = true;
            });
            while (!isGo)
            {

            }
            return result;
        }
    }
    private void ReplaceDocument(string text, int start = 0, int length = int.MinValue)
    {
        if (Document is null)
            return;

        if (length == int.MinValue)
        {
            length = Document.TextLength;
        }
        string docText = GetDocumentText();
        if (start == 0 && length == docText.Length && text == docText)
            return;

        isChangingText = true;
        if (UIDispatcher.CheckAccess())
        {
            ReplaceAction(text, start, length);
        }
        else
        {
            UIDispatcher.Post(() =>
            {
                ReplaceAction(text, start, length);
            });
        }

        void ReplaceAction(string text, int start, int length)
        {
            Document.BeginUpdate();
            Document.Remove(start, length);
            Document.Insert(start, text ?? string.Empty);
            Document.EndUpdate();
        }
        isChangingText = false;
    }

}
