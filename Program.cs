﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace SCLOCUA
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{EA6A248E-8F44-4C82-92F6-03F6A055E637}");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Створення форми і об'єкта NotifyIcon
                Form1 mainForm = new Form1();
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.Icon = mainForm.Icon;
                notifyIcon.Text = "Українізатор Star Citizen";
                notifyIcon.DoubleClick += (sender, e) =>
                {
                    // Показати форму, коли користувач подвійно клацне на іконці трею
                    mainForm.Show();
                    mainForm.WindowState = FormWindowState.Normal;
                };

                // Додавання контекстного меню для іконки у треї
                ContextMenu contextMenu = new ContextMenu();
                MenuItem startupMenuItem = new MenuItem("Запускати при старті");
                bool isStartupEnabled = IsStartupEnabled();
                startupMenuItem.Checked = isStartupEnabled;
                startupMenuItem.Click += (sender, e) =>
                {
                    // Зміна стану пункта меню "Запускати при старті"
                    SetStartup(!isStartupEnabled);
                    startupMenuItem.Checked = !isStartupEnabled;
                };
                contextMenu.MenuItems.Add("Відкрити", (sender, e) =>
                {
                    mainForm.Show();
                    mainForm.WindowState = FormWindowState.Normal;
                });
                contextMenu.MenuItems.Add(startupMenuItem);
                contextMenu.MenuItems.Add("Вихід", (sender, e) =>
                {
                    Application.Exit();
                });
                notifyIcon.ContextMenu = contextMenu;

                // Додавання обробника події Resize для форми
                mainForm.Resize += (sender, e) =>
                {
                    if (mainForm.WindowState == FormWindowState.Minimized)
                    {
                        mainForm.Hide();
                        notifyIcon.Visible = true; // Показати іконку у треї
                    }
                };

                // Запуск програми та відображення головної форми
                Application.Run(mainForm);
                mutex.ReleaseMutex();
            }
        }

        static string AppName = "SCLocalizationUA";
        static string RegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        static bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true))
            {
                return key.GetValue(AppName) != null;
            }
        }

        static void SetStartup(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true))
            {
                if (enable)
                {
                    key.SetValue(AppName, Application.ExecutablePath);
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }
            }
        }
    }
}
