# The Daily Market - Grocery Store 🍎

A full-stack ASP.NET Core MVC web application for managing a grocery store with user login, product browsing, shopping cart, and admin dashboard.

## 🔍 Project Overview

This application allows users to:
- Browse products by category
- Add products to a shopping cart (only when logged in)
- View, update, or clear their cart
- Register and log in securely
- Admins can manage (Create/Read/Update/Delete) products, categories, and suppliers via a dashboard

## 💡 Extra Feature

✅ **Role-Based Navigation:** Admins and Users see different options in the navbar.

✅ **Cart Security:** Cart data is stored per authenticated user, preventing unauthorized access or data mixing.

✅ **API Integration:** Used fetch API for `Add to Cart`, making cart updates dynamic without full page reload.

## 🧩 CRUD Functionality Showcase

| Entity     | Create | Read | Update | Delete |
|------------|--------|------|--------|--------|
| Products   | ✅      | ✅    | ✅      | ✅      |
| Categories | ✅      | ✅    | ✅      | ✅      |
| Suppliers  | ✅      | ✅    | ✅      | ✅      |
| Cart Items | ✅      | ✅    | ✅      | ✅      |

## ⚠️ Challenges & Fixes

> "One big challenge I faced was making sure each user's cart was private and secure. At first, I used a hardcoded test ID, so all users shared the same cart by mistake. I fixed this by linking the cart to the logged-in user's ID and blocking access for guests."

## 🎥 Video Demo

[Watch Presentation Video](https://padlet.com/christinebittle/passion-project-async-presentations-jn16e3wljfv2nju7/wish/j40PQDnwPzezWvXB)

## 📁 Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- Identity (for authentication & authorization)
- Bootstrap 5
- LINQ & C#
- SQL Server LocalDB

## 👤 Roles

- Admin: Can manage store data.
- User: Can browse and purchase items.

---

Feel free to clone this project and customize it for your needs.

