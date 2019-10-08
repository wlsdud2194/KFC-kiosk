﻿using Kfc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KfcKiosk
{
    public class PayArgs : EventArgs
    {
        public Seat selectedSeat { get; set; }
    }
    public partial class PaymentCtrl : UserControl, INotifyPropertyChanged
    {
        public delegate void OnPayEventHandler(object sender, PayArgs args);
        public event OnPayEventHandler PayEvent;

        public Seat SelectedSeat { get; set; }
        private List<Food> orderList = new List<Food>();

        private int total = 0;
        public int Total
        {
            get => total;
            set
            {
                total = value;
                NotifyPropertyChanged(nameof(Total));
            }
        }

        public PaymentCtrl()
        {
            InitializeComponent();
            this.Loaded += PaymentCtrl_Loaded;
        }

        private void PaymentCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            //App.foodData.Load();
            LoadMenu("All");
            totalPrice.DataContext = this;
        }

        private void Prev_Ctrl(object sender, RoutedEventArgs e)
        {
            PayArgs args = new PayArgs();

            args.selectedSeat = this.SelectedSeat;

            if (PayEvent != null)
            {
                PayEvent(this, args);
            }
        }

        private void LoadMenu(string selectedCategory)
        {
            if (selectedCategory == null) selectedCategory = "All";

            switch (selectedCategory)
            {
                case "Burger":
                    SetMenu("Burger");
                    break;

                case "Chicken":
                    SetMenu("Chicken");
                    break;

                case "Drink":
                    SetMenu("Drink");
                    break;

                case "Snack":
                    SetMenu("Snack");
                    break;

                default:
                    SetMenu("All");
                    break;
            }
        }

        private void SetMenu(string selectedCategory)
        {
            List<Food> foodList = new List<Food>();

            foreach (Food food in App.foodData.lstMenu)
            {
                if (food.Category.ToString().Equals(selectedCategory) || selectedCategory.Equals("All"))
                {
                    foodList.Add(food);
                }
            }

            lvMenu.ItemsSource = foodList;
        }

        private void LvCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = ((ListBoxItem)(lvCategory.SelectedItem)).Content.ToString();
            LoadMenu(selectedCategory);
        }

        private void LvMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Food selectedMenu = ((Food)lvMenu.SelectedItem);

            if (selectedMenu == null) return;

            ResetTime();

            if (!(orderList.Contains(selectedMenu)))
            {
                selectedMenu.Count = 1;
                orderList.Add(selectedMenu);

                Total += selectedMenu.Price;
            }

            lvOrdered.ItemsSource = orderList;
            lvOrdered.Items.Refresh();

            lvMenu.SelectedItem = null;
        }

        private void ResetTime()
        {
            leastOrderTime.Text = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
        }

        private void Counting(object sender, RoutedEventArgs e)
        {
            //UIElementCollection parentTextBlock = (((((sender as FrameworkElement).Parent) as FrameworkElement).Parent) as Grid).Children;
            UIElementCollection siblingEl = (((sender as FrameworkElement).Parent) as Grid).Children;
            string foodName = (siblingEl[1] as TextBlock).Text;
            string content = (sender as Button).Content.ToString();

            foreach (Food menu in orderList)
            {
                if (menu.Name.Equals(foodName))
                {
                    if (content.Equals("+"))
                    {
                        menu.Count++;
                        Total += menu.Price;
                    }
                    else if (content.Equals("-"))
                    {

                        menu.Count--;
                        Total -= menu.Price;

                        if (menu.Count < 1) orderList.Remove(menu);
                    }
                    break;
                }
            }
            lvOrdered.Items.Refresh();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            orderList.Clear();
            Total = 0;
            lvOrdered.Items.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}