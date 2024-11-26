# QuickBite Web Application

**COMP 3415 – Software Engineering**  
**Last Updated: December 2, 2024**  

---

## Project Overview

**QuickBite** is a web-based local food delivery platform designed to connect consumers with their favorite restaurants. It enhances convenience for users while providing restaurants with access to new revenue streams and customer bases. The system includes functional dashboards for consumers, restaurant partners, delivery drivers, and administrators.

---

## Table of Contents

1. [System Requirements](#system-requirements)
2. [Installation Guide](#installation-guide)
3. [User Manual](#user-manual)

---

## System Requirements

To deploy and run QuickBite successfully, ensure your environment meets the following requirements:

### Development Stack:
- **ASP.NET Core**: Latest version ([Download here](https://dotnet.microsoft.com/download/dotnet))
- **Node.js**: Version 23.3.0 or higher ([Download here](https://nodejs.org/))
- **PostgreSQL**: Version 17.2 or higher ([Download here](https://www.postgresql.org/download/))

### UI/UX Design:
- **Tailwind CSS**: Version 3.0 ([Documentation](https://tailwindcss.com/docs/installation))

### Cloud Hosting:
- **Render** (for PostgreSQL hosting)
- **IONOS** (for ASP.NET hosting)

### Additional Tools:
- **Jira** (Project management)
- **Figma** (UI/UX design)
- **Discord** (Team communication)

---

## Installation Guide

### 1. Clone the Repository:
```bash
git clone https://github.com/JaskarnDhillon/QuickBiteCOMP3415/tree/master.git
cd QuickBite
```

### 2. Install Dependencies
Ensure you have **Node.js** and **npm** installed. Then, run:
```bash
npm install
```

### 3. Install Tailwind CSS:
```bash
npm install -D tailwindcss
npx tailwindcss init
```

### 4. Setup PostgresSQL Database:

1. **Install PostgreSQL** if not already installed.  
   Download from [PostgreSQL official site](https://www.postgresql.org/download/).

2. **Configure the Connection String:**
   Update your `appsettings.json` with the following connection details:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=[hostname];Port=5432;Database=[database];Username=[username];Password=[password];SSL Mode=Require;Trust Server Certificate=true"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }

### 6. Run the Application

Once all dependencies are installed and your PostgreSQL database is configured, you can run the application.

1. **Run the Application:**
   In the terminal, navigate to the project directory and run the following command:
   ```bash
   dotnet watch run
   ```

2. **Access the Application:**
   - After the application starts, open your browser and go to `https://localhost:5188` (by default).
   - You should now be able to interact with the QuickBite platform.

3. **Verify the Application:**
   - Test basic functionality like signing up as a customer, restaurant partner, or driver.
   - Ensure that all components (frontend and backend) are working together seamlessly.

---

## User Manual

### 1. User Roles

QuickBite includes several user roles, each with specific permissions and functionality:

- **Customer**: Can browse restaurants, place orders, track deliveries, and manage account details.
- **Restaurant Partner**: Can manage the menu, view orders, and update order statuses.
- **Delivery Driver**: Can view assigned delivery tasks, update delivery status, and manage deliveries.
- **Administrator**: Manages system-wide configurations, user roles, and overall system maintenance.

### 2. Signing Up and Logging In

- **Customer**:
  1. Navigate to the QuickBite homepage.
  2. Click on the "Sign Up" button.
  3. Fill in personal information and create an account.
  4. Log in using your newly created credentials.

- **Restaurant Partner/Delivery Driver**:
  - If you're an existing restaurant partner or driver, use your provided credentials to log in.
  - If you're new, register as a restaurant.

### 3. Placing an Order (Customer)

1. Browse the list of available restaurants.
2. Select your desired restaurant and menu items.
3. Add items to your cart.
4. Review your cart and proceed to checkout.
5. Choose delivery options and payment methods.
6. Place the order. You'll receive notifications on the order status and delivery progress.

### 4. Managing Orders (Restaurant Partner)

1. Log in to your restaurant dashboard.
2. View incoming orders and their details.
3. Update the status of orders (e.g., approved, ready).
4. Manage the menu and update item availability as needed.

### 5. Managing Deliveries (Delivery Driver)

1. Log in to your delivery driver dashboard.
2. View assigned deliveries.
3. Update the delivery status as you progress (e.g., out-for-delivery, delivered).
4. Complete deliveries and report any issues.

---

## Conclusion

The **QuickBite** platform is designed to make food delivery convenient and efficient for all users, including customers, restaurant partners, delivery drivers, and administrators. By following the installation guide and user manual, you'll be able to set up the platform and begin using it in no time. Future updates will continue to enhance its functionality and usability.
