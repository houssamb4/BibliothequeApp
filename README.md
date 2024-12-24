# BibliothequeApp

BibliothequeApp is a C# application for managing a library system. The project uses a SQL Server database to store information about authors and books, allowing basic CRUD operations and ensuring data integrity with foreign keys and constraints.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Database Schema](#database-schema)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

---

## Project Overview

BibliothequeApp provides a simple way to manage books and authors in a library. It connects to a SQL Server database and allows adding, updating, deleting, and viewing books and authors. Data consistency is maintained through foreign keys and validation rules.

---

## Database Schema

The project uses a SQL Server database named `Bibliotheque` with the following tables:

### `Auteurs` (Authors)
- **Id**: Primary key, auto-increment integer
- **Nom**: Author's name (nvarchar, required)

### `Livres` (Books)
- **Id**: Primary key, auto-increment integer
- **Titre**: Book title (nvarchar, required, unique)
- **Id_Auteur**: Foreign key referencing `Auteurs(Id)`
- **Nb_Pages**: Number of pages (integer, must be > 0)
- **Prix**: Price of the book (decimal, must be ≥ 0)

Constraints:
- `Livres.Id_Auteur` references `Auteurs(Id)`  
- `Livres.Nb_Pages > 0`  
- `Livres.Prix ≥ 0`  

---

## Features

- Manage authors: Add, update, delete, and list authors
- Manage books: Add, update, delete, and list books
- Ensure data consistency with foreign key constraints
- Validation for page numbers and book prices
- SQL Server database integration

---

## Technologies Used

- SQL Server
- T-SQL for database scripts
- C# 

---

## Installation

1. Clone the repository:

```bash
git clone https://github.com/houssamb4/BibliothequeApp.git
