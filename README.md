 # 📦 Product Order Management System

A comprehensive **ASP.NET MVC 5** web application for managing product inventory, customer information, orders, and business analytics. Built with Entity Framework 6, SQL Server, Bootstrap 4, and jQuery.

## 🎯 Features

### 📊 Dashboard & Analytics
- Real-time business metrics (Total Products, Revenue, Orders, Customers)
- Average order value tracking
- Recent orders monitoring (Last 7 days)
- Top product categories analysis

### 🏷️ Product Management
- Create, Read, Update, Delete products
- Product categorization system
- Image upload support
- Multiple unit types (Piece, kg, g, Liter, ml, Box, Dozen, Pack, Bundle, Meter, Cm, Hour, Day, Month)
- Inventory tracking with stock validation
- Active/Inactive product status
- Product search and category filtering

### 🗂️ Category Management
- Organize products into categories
- Category descriptions
- View category statistics

### 👥 Customer Management
- Customer information tracking
- Auto-customer creation on order placement
- Contact details management (Name, Phone, Address)
- Customer order history

### 📝 Order Management
- Create multi-item orders with real-time stock validation
- Prevent overselling with inventory checks
- Automatic customer creation/update
- Order tracking with date and total amount
- Transaction-based processing for data integrity
- Order filtering and sorting

---

## 🛠️ Technology Stack

| Technology | Version |
|---|---|
| **ASP.NET MVC** | 5.2.9 |
| **Entity Framework** | 6.5.1 |
| **.NET Framework** | 4.8 |
| **SQL Server** | Database backend |
| **Bootstrap** | 4.5.2 |
| **jQuery** | 3.5.1 |
| **jQuery Validation** | 1.19.5 |
| **Newtonsoft.Json** | 13.0.1 |

---

## 📁 Project Structure
ProductOrderManagement/
├── Controllers/
│ ├── HomeController.cs # Dashboard & Analytics
│ ├── ProductController.cs # Product CRUD operations
│ ├── ProductCategoryController.cs
│ └── OrderController.cs # Order management
├── Models/
│ ├── Product.cs
│ ├── Order.cs
│ ├── OrderDetail.cs
│ ├── Customer.cs
│ ├── ProductCategory.cs
│ ├── MyEntityDataModel.edmx # Entity Framework model
│ └── ViewModels/
├── Views/
│ ├── Home/
│ ├── Product/
│ ├── ProductCategory/
│ └── Order/
├── Content/ # CSS files (Bootstrap)
├── Scripts/ # jQuery and Bootstrap JS
├── Images/ # Product images
├── App_Start/
│ ├── RouteConfig.cs
│ ├── BundleConfig.cs
│ └── FilterConfig.cs
└── Web.config # Configuration file

📖 Usage Guide
Creating a Product

Navigate to Products → Create New
Enter product details (name, category, price, quantity)
Upload product image
Save
Processing an Order

Go to Orders → Create Order
Select or create customer
Add products and quantities
System validates stock
Submit order
Viewing Analytics

Go to Dashboard (Home page)
View key metrics and charts
Monitor recent orders
📊 Database Models
Product - Product catalog with pricing and stock
ProductCategory - Category classification
Order - Customer orders with total amounts
OrderDetail - Order line items
Customer - Customer information and contact details
📝 License
MIT License - Free to use and modify

🤝 Support
For issues or questions, open an issue on GitHub.

Happy Coding! 🚀

