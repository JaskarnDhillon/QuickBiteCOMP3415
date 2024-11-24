# Software Requirements Specification for QuickBite – a Local Food Delivery System

## 1. Introduction

### 1.1 Purpose
The purpose of this document is to specify the requirements, specifications, and scope of QuickBite – a local food delivery system application. It details the functional and non-functional requirements, and application constraints.

### 1.2 Scope
QuickBite will provide functionality for the following:

- Consumer registration and account management.
- Access to the food catalog to create, view, and cancel food deliveries.
- Restaurant registration and account management.
- Access to manage the food catalog, set pricing, and communicate with consumers for their location(s).
- Administrative access for complete control over the application.

The application will be used by consumers, restaurant management, and app administrators.

### 1.3 Definitions, Acronyms, and Abbreviations
- **Consumer**: A user who accesses the food catalog to make purchases.
- **Partner**: Restaurant owners and staff who manage their food catalog, set pricing, and communicate with consumers.
- **Administrator**: Any member of the QuickBite development team with administrative-level privileges.

### 1.4 References
- **Development Stack**: ASP.NET, Tailwind CSS
- **Database**: PostgreSQL
- **Cloud Hosting**: Render, IONOS
- **UI/UX Design**: Figma
- **Communication**: Jira, Discord

## 2. Overall Description

### 2.1 Product Perspective
QuickBite is a web-based platform that lets consumers order from all their favorite restaurants from the comfort of their home. It also provides restaurants with access to a new revenue stream, alongside a new customer base.

### 2.2 Product Features
Key features of QuickBite include:
- Distinct consumer and partner sign-up.
- Consumer ability to order, view, and cancel food deliveries.
- Partner ability to confirm orders.
- Partner ability to customize their food catalog, business hours, and more.
- Administrator ability to oversee the entire application with no role-based limitations.
- Direct communication channels between restaurant partners and consumers.

### 2.3 User Classes and Characteristics
- **Consumers**: Primary users who will order, view, and cancel food deliveries. They require a simple, easy-to-use interface.
- **Partners**: Users responsible for managing their restaurant(s), including food catalogs, hours of operation, and more. They require a functional and straightforward interface.
- **Administrators**: Users who manage the entire application. They require a functional and responsive interface; UX design is not a priority.

### 2.4 Operating Environment
QuickBite will be a web-based application, built using ASP.NET and PostgreSQL (hosted on Render.com). The application will be accessible and responsive on all devices via modern browsers (Chrome, Firefox, Edge, Safari, etc.).

### 2.5 Assumptions and Dependencies
- Internet access and cookies will be required for the application to function.
- The application will use third-party mapping software (e.g., Mapbox) for tracking deliveries.

## 3. Functional Requirements

### 3.1 Consumer Registration Module
- **FR-1**: Consumers can register and manage their accounts.
- **FR-2**: Consumers can search, filter, and browse the food catalog.
- **FR-3**: Consumers can order food deliveries with customization.
- **FR-4**: Consumers can view and cancel active food deliveries.
- **FR-5**: Consumers can communicate with restaurant partners.

### 3.2 Restaurant Registration Module
- **FR-6**: Partners can register and manage their accounts.
- **FR-7**: Partners can create and manage food catalogs, restaurant hours, and more.
- **FR-8**: Partners can confirm and cancel food deliveries.
- **FR-9**: Partners can communicate with consumers.
- **FR-10**: Partners can view and export order data.

### 3.3 Administrative Control
- **FR-11**: Administrators can edit and delete consumer and partner accounts.
- **FR-12**: Administrators can view both consumer and partner data.
- **FR-13**: Administrators can communicate with consumers and partners.
- **FR-14**: Administrators can edit and delete food catalogs.

## 4. Non-Functional Requirements

### 4.1 Performance Requirements
- **NFR-1**: The application should handle up to 5000 users simultaneously without performance degradation.
- **NFR-2**: Application loading times should not exceed 3 seconds.

### 4.2 Security Requirements
- **NFR-3**: Users must log in for most functionality; anonymous users may view food catalogs but cannot order delivery.
- **NFR-4**: All passwords and payment information will be encrypted.

### 4.3 Usability Requirements
- **NFR-5**: The application will have a simple, easy-to-use interface for all user levels.
- **NFR-6**: Light mode will be integrated, with the ability to add dark mode in future versions.

### 4.4 Availability Requirements
- **NFR-7**: The application will have 99.9% uptime, excluding scheduled maintenance.
- **NFR-8**: It will be available on all modern browsers and responsive for all screen sizes.

### 4.5 Backup and Recovery
- **NFR-9**: Automatic daily backups will be retained for at least 7 days.
- **NFR-10**: Lost data due to user error may not be recoverable.

## 5. Constraints
The QuickBite web application will be fully accessible 24/7 (except during scheduled maintenance), but food delivery will depend on user location and restaurant hours. Third-party map software will be supported based on user preferences and permissions.

## 6. Appendices
- [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)
- [IONOS](https://www.ionos.com/)
- [Tailwind CSS](https://tailwindcss.com/)
- [PostgreSQL](https://www.postgresql.org/)
- [Render](https://render.com/)
- [Figma](https://www.figma.com/)
- [Jira](https://www.atlassian.com/software/jira)
- [Discord](https://discord.com/)
- [Google Chrome](https://www.google.com/chrome/)
- [Firefox](https://www.mozilla.org/firefox/)
- [Microsoft Edge](https://www.microsoft.com/edge)
- [Safari](https://www.apple.com/safari/)
- [Mapbox](https://www.mapbox.com/)
