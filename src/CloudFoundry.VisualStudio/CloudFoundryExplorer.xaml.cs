﻿using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CloudFoundry.VisualStudio.Forms;
using CloudFoundry.VisualStudio.Model;
using CloudFoundry.VisualStudio.TargetStore;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.Threading;

namespace CloudFoundry.VisualStudio
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class CloudFoundryExplorer : UserControl
    {
        public CloudFoundryExplorer()
        {
            InitializeComponent();
            ReloadTargets();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void treeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem == null)
            {
                return;
            }

            var cloudItemAction = menuItem.Tag as CloudItemAction;

            if (cloudItemAction != null)
            {
                cloudItemAction.CloudItem.ExecutingBackgroundAction = true;

                cloudItemAction.Click.Invoke().ContinueWith(async (antecedent) =>
                {
                    cloudItemAction.CloudItem.ExecutingBackgroundAction = false;

                    if (antecedent.IsFaulted)
                    {
                        var errorMessages = new List<string>();
                        ErrorFormatter.FormatExceptionMessage(antecedent.Exception, errorMessages);
                        MessageBoxHelper.DisplayError(string.Join(Environment.NewLine, errorMessages));
                    }
                    else
                    {
                        switch (cloudItemAction.Continuation)
                        {
                            case CloudItemActionContinuation.RefreshChildren:
                                await cloudItemAction.CloudItem.RefreshChildren();
                                break;
                            case CloudItemActionContinuation.RefreshParent:
                                if (cloudItemAction.CloudItem.ItemType == CloudItemType.Target)
                                {
                                    ThreadHelper.Generic.Invoke(() => this.ReloadTargets());
                                }
                                else
                                {
                                    await cloudItemAction.CloudItem.Parent.RefreshChildren();
                                }
                                break;
                            case CloudItemActionContinuation.None:
                            default:
                                break;
                        }
                    }
                });
            }
        }

        private void ExplorerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CloudItem item = e.NewValue as CloudItem;
            if (item != null)
            {
                ListView.ItemsSource = GetItemList(item);
            }
        }

        private void ExplorerTree_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                e.Handled = true;
            }
        }

        private void AddTargetButton_Click(object sender, RoutedEventArgs e)
        {
            using (var loginForm = new LoginWizardForm())
            {
                var result = loginForm.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var target = loginForm.CloudTarget;

                    if (target != null)
                    {
                        CloudTargetManager.SaveTarget(target);
                        CloudCredentialsManager.Save(target.TargetUrl, target.Email, loginForm.Password);
                        ReloadTargets();
                    }
                }
            }
        }


        private void ReloadTargets()
        {
            this.ExplorerTree.Items.Clear();

            var targets = CloudTargetManager.GetTargets();

            foreach (var target in targets)
            {
                ExplorerTree.Items.Add(new CloudFoundryTarget(target));
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ExplorerTree.SelectedValue as CloudItem;

            if (selectedItem != null)
            {
                selectedItem.RefreshChildren().Forget();
            }
        }

        private SortedList<string, object> GetItemList(CloudItem item)
        {
            var list = new SortedList<string, object>();
            if (item == null)
            {
                return list;
            }

            PropertyInfo[] properties = item.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                var browsable = pi.GetCustomAttribute<BrowsableAttribute>();
                if ((browsable != null) && (!browsable.Browsable)) continue;

                var displayName = pi.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                {
                    list.Add(displayName.DisplayName, pi.GetValue(item, null));
                }
                else
                {
                    list.Add(pi.Name, pi.GetValue(item, null));
                }
            }

            return list;
        }

        private void OnSortClick(object sender, RoutedEventArgs e)
        {
            var list = ListView.ItemsSource as IEnumerable<KeyValuePair<string, object>>;
            if (list != null)
            {
                ListView.ItemsSource = list.Reverse();
            }
        }
    }
}