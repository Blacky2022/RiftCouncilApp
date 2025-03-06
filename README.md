# **RiftCouncil App - A League of Legends Suggestion Platform**

RiftCouncil is a **community-driven suggestion platform** designed for **League of Legends** players. Built with **.NET 6 Blazor WebAssembly** and **MongoDB**, the application allows users to **submit suggestions** for game improvements, champion balances, or quality-of-life changes. Administrators can then **review, approve, or reject** these suggestions. 

The app integrates **Azure B2C for authentication** for user authentication and authorization.

## **Features**
- **User Authentication** – Secure login with **Azure B2C**.
- **Suggestion Submission** – Players can submit ideas for **champion balance, items, gameplay changes, or new features**.
- **Moderation System** – Admins review, approve, or reject community suggestions.
- **League of Legends-Focused** – Tailored for players to express their thoughts on **game mechanics, esports changes, or client improvements**.

## **Prerequisites**
Before running the project, ensure you have:
- **.NET 6 SDK**
- **MongoDB**
- **Azure B2C Tenant**

## **Getting Started**
### **Clone the repository**
```bash
git clone <your-repository-url>
```

### **Navigate to the project directory**
```bash
cd RiftCouncilApp
```

### **Restore dependencies**
```bash
dotnet restore
```

### **Configure MongoDB and Azure B2C Settings**
1. Open the **`appsettings.json`** file.
2. Set the **MongoDbConnection** to your MongoDB connection string.
3. Update the **AzureAdB2c** section with your **Azure B2C ClientId** and **Instance**.

## **Usage**
Once deployed, users can:
- **Log in** using Azure B2C authentication.
- **Submit suggestions** related to **League of Legends**.
- **Admins** can **review, approve, or reject** suggestions.
- Community ideas can be **highlighted for discussion**.

