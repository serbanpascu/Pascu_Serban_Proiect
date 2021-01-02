using System;
using System.Collections.Generic;
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
using CakeShopModel;
using System.Data.Entity;
using System.Data;

namespace Pascu_Serban_Proiect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        CakeShopEntitiesModel ctx = new CakeShopEntitiesModel();

        CollectionViewSource customerViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();
        Binding txtAgeBinding = new Binding();

        CollectionViewSource productViewSource;
        Binding txtTypeBinding = new Binding();
        Binding txtFlavorBinding = new Binding();

        CollectionViewSource customerOrdersViewSource;
        Binding txtCustomer = new Binding();
        Binding txtProduct = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtFirstNameBinding.Path = new PropertyPath("FristName");
            txtLastNameBinding.Path = new PropertyPath("LastName");
            txtAgeBinding.Path = new PropertyPath("Age");
            txtTypeBinding.Path = new PropertyPath("Type");
            txtFlavorBinding.Path = new PropertyPath("Flavor");
            txtCustomer.Path = new PropertyPath("Customer");
            txtProduct.Path = new PropertyPath("Product");

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            typeTextBox.SetBinding(TextBox.TextProperty, txtTypeBinding);
            flavorTextBox.SetBinding(TextBox.TextProperty, txtFlavorBinding);
            cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbProduct.SetBinding(ComboBox.TextProperty, txtProduct);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource =
                ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();

            customerOrdersViewSource =
                ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();

            productViewSource =
                ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = ctx.Products.Local;
            ctx.Products.Load();

            cmbCustomer.ItemsSource = ctx.Customers.Local;
            cmbCustomer.SelectedValuePath = "CustId";

            cmbProduct.ItemsSource = ctx.Products.Local;
            cmbProduct.SelectedValuePath = "DesertId";

            BindDataGrid();
        }

        private void btnNewC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;

            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEditC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;

            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);

            SetValidationBinding();
        }

        private void btnDeleteC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;

            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnSaveC_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }
            SetValidationBinding();
        }

        private void btnCancelC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewC.IsEnabled = true;
            btnEditC.IsEnabled = true;
            btnDeleteC.IsEnabled = true;
            btnSaveC.IsEnabled = false;
            btnCancelC.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            btnPrevC.IsEnabled = true;
            btnNextC.IsEnabled = true;

            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }

        private void btnPrevC_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextC_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNewP_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewP.IsEnabled = false;
            btnEditP.IsEnabled = false;
            btnDeleteP.IsEnabled = false;

            btnSaveP.IsEnabled = true;
            btnCancelP.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            btnPrevP.IsEnabled = false;
            btnNextP.IsEnabled = false;

            typeTextBox.IsEnabled = true;
            flavorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(typeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(flavorTextBox, TextBox.TextProperty);
            typeTextBox.Text = "";
            flavorTextBox.Text = "";
            Keyboard.Focus(typeTextBox);
        }

        private void btnEditP_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempType = typeTextBox.Text.ToString();
            string tempFlavor = flavorTextBox.Text.ToString();

            btnNewP.IsEnabled = false;
            btnEditP.IsEnabled = false;
            btnDeleteP.IsEnabled = false;

            btnSaveP.IsEnabled = true;
            btnCancelP.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            btnPrevP.IsEnabled = false;
            btnNextP.IsEnabled = false;

            typeTextBox.IsEnabled = true;
            flavorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(typeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(flavorTextBox, TextBox.TextProperty);
            typeTextBox.Text = tempType;
            flavorTextBox.Text = tempFlavor;
            Keyboard.Focus(typeTextBox);

            SetValidationBinding();
        }

        private void btnDeleteP_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempType = typeTextBox.Text.ToString();
            string tempFlavor = flavorTextBox.Text.ToString();

            btnNewP.IsEnabled = false;
            btnEditP.IsEnabled = false;
            btnDeleteP.IsEnabled = false;

            btnSaveP.IsEnabled = true;
            btnCancelP.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            btnPrevP.IsEnabled = false;
            btnNextP.IsEnabled = false;

            BindingOperations.ClearBinding(typeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(flavorTextBox, TextBox.TextProperty);
            typeTextBox.Text = tempType;
            flavorTextBox.Text = tempFlavor;
        }

        private void btnSaveP_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;
            if (action == ActionState.New)
            {
                try
                {
                    product = new Product()
                    {
                        Type = typeTextBox.Text.Trim(),
                        Flavor = flavorTextBox.Text.Trim(),
                    };
                    ctx.Products.Add(product);
                    productViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewP.IsEnabled = true;
                btnEditP.IsEnabled = true;
                btnDeleteP.IsEnabled = true;
                btnSaveP.IsEnabled = false;
                btnCancelP.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                btnPrevP.IsEnabled = true;
                btnNextP.IsEnabled = true;
                typeTextBox.IsEnabled = false;
                flavorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    product.Type = typeTextBox.Text.Trim();
                    product.Flavor = flavorTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
                productViewSource.View.MoveCurrentTo(product);
                btnNewP.IsEnabled = true;
                btnEditP.IsEnabled = true;
                btnDeleteP.IsEnabled = true;
                btnSaveP.IsEnabled = false;
                btnCancelP.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                btnPrevP.IsEnabled = true;
                btnNextP.IsEnabled = true;
                typeTextBox.IsEnabled = false;
                flavorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
                productViewSource.View.MoveCurrentTo(product);
                btnNewP.IsEnabled = true;
                btnEditP.IsEnabled = true;
                btnDeleteP.IsEnabled = true;
                btnSaveP.IsEnabled = false;
                btnCancelP.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                btnPrevP.IsEnabled = true;
                btnNextP.IsEnabled = true;
                typeTextBox.IsEnabled = false;
                flavorTextBox.IsEnabled = false;

                typeTextBox.SetBinding(TextBox.TextProperty, txtTypeBinding);
                flavorTextBox.SetBinding(TextBox.TextProperty, txtFlavorBinding);
            }
            SetValidationBinding();
        }

        private void btnCancelP_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewP.IsEnabled = true;
            btnEditP.IsEnabled = true;
            btnDeleteP.IsEnabled = true;
            btnSaveP.IsEnabled = false;
            btnCancelP.IsEnabled = false;
            productDataGrid.IsEnabled = true;
            btnPrevP.IsEnabled = true;
            btnNextP.IsEnabled = true;

            typeTextBox.IsEnabled = false;
            flavorTextBox.IsEnabled = false;

            typeTextBox.SetBinding(TextBox.TextProperty, txtTypeBinding);
            flavorTextBox.SetBinding(TextBox.TextProperty, txtFlavorBinding);
        }

        private void btnPrevP_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextP_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToNext();
        }

        private void btnNewO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;

            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrevO.IsEnabled = false;
            btnNextO.IsEnabled = false;

            cmbCustomer.IsEnabled = true;
            cmbProduct.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomer, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProduct, ComboBox.TextProperty);
            cmbCustomer.Text = "";
            cmbProduct.Text = "";
            Keyboard.Focus(cmbCustomer);
        }

        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempCustomer = cmbCustomer.Text.ToString();
            string tempProduct = cmbProduct.Text.ToString();

            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;

            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrevO.IsEnabled = false;
            btnNextO.IsEnabled = false;

            cmbCustomer.IsEnabled = true;
            cmbProduct.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomer, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProduct, ComboBox.TextProperty);
            cmbCustomer.Text = tempCustomer;
            cmbProduct.Text = tempProduct;
            Keyboard.Focus(cmbCustomer);

            SetValidationBinding();
        }

        private void btnDeleteO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempCustomer = cmbCustomer.Text.ToString();
            string tempProduct = cmbProduct.Text.ToString();

            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;

            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrevO.IsEnabled = false;
            btnNextO.IsEnabled = false;

            BindingOperations.ClearBinding(cmbCustomer, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbProduct, TextBox.TextProperty);
            cmbCustomer.Text = tempCustomer;
            cmbProduct.Text = tempProduct;
        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomer.SelectedItem;
                    Product product = (Product)cmbProduct.SelectedItem;

                    order = new Order()
                    {
                        CustId = customer.CustId,
                        DesertId = product.DesertId
                    };
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbProduct.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomer.SelectedValue.ToString());
                        editedOrder.DesertId = Int32.Parse(cmbProduct.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                BindDataGrid();
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                customerOrdersViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(order);
                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbProduct.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Sucessfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(order);
                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbProduct.IsEnabled = false;

                cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
                cmbProduct.SetBinding(ComboBox.TextProperty, txtProduct);
            }
            SetValidationBinding();
        }

        private void btnCancelO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewO.IsEnabled = true;
            btnEditO.IsEnabled = true;
            btnDeleteO.IsEnabled = true;
            btnSaveO.IsEnabled = false;
            btnCancelO.IsEnabled = false;
            ordersDataGrid.IsEnabled = true;
            btnPrevO.IsEnabled = true;
            btnNextO.IsEnabled = true;

            cmbCustomer.IsEnabled = false;
            cmbProduct.IsEnabled = false;

            cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbProduct.SetBinding(ComboBox.TextProperty, txtProduct);
        }

        private void btnPrevO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join prod in ctx.Products on ord.DesertId equals prod.DesertId
                             select new { ord.OrderId, ord.DesertId, ord.CustId, cust.FirstName, 
                             cust.LastName, prod.Type, prod.Flavor };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FristName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            lastNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);

            Binding typeValidationBinding = new Binding();
            typeValidationBinding.Source = productViewSource;
            typeValidationBinding.Path = new PropertyPath("Type");
            typeValidationBinding.NotifyOnValidationError = true;
            typeValidationBinding.Mode = BindingMode.TwoWay;
            typeValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            typeValidationBinding.ValidationRules.Add(new StringNotEmpty());
            typeTextBox.SetBinding(TextBox.TextProperty, typeValidationBinding);

            Binding flavorValidationBinding = new Binding();
            flavorValidationBinding.Source = productViewSource;
            flavorValidationBinding.Path = new PropertyPath("FristName");
            flavorValidationBinding.NotifyOnValidationError = true;
            flavorValidationBinding.Mode = BindingMode.TwoWay;
            flavorValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            flavorValidationBinding.ValidationRules.Add(new StringNotEmpty());
            flavorTextBox.SetBinding(TextBox.TextProperty, flavorValidationBinding);
        }
    }
}