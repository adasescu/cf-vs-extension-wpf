﻿// -----------------------------------------------------------------------
// <copyright file="MessageBoxHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CloudFoundry.VisualStudio.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MessageBoxHelper
    {
        public static DialogResult ErrorWithMessageBox(string message)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult ErrorWithMessageBox(Exception ex)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(ex.Message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult ErrorWithMessageBox(string message, Exception ex)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message + "\r\n\r\n" + ex.Message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult DisplayError(string message)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult DisplayError(string message, Exception ex)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message + "\r\n\r\n" + ex.ToString(), Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult DisplayError(Exception ex)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(ex.ToString(), Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public static DialogResult DisplayInfo(string message)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        public static DialogResult DisplayWarning(string message)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message, Logger.EventSource, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
        }

        public static DialogResult WarningQuestion(string message)
        {
            return ThreadHelper.Generic.Invoke<DialogResult>(() =>
            {
                return MessageBox.Show(message, Logger.EventSource, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            });
        }

    }
}
