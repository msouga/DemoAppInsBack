-- init.sql
-- Script de inicialización para DemoDB

-- Crear la base de datos
CREATE DATABASE DemoDB;
GO

-- Usar la base de datos
USE DemoDB;
GO

-- Crear una tabla simple de productos
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

-- Insertar algunos datos de prueba
INSERT INTO Productos (Nombre, Precio, Stock) VALUES
('Mouse inalámbrico', 49.90, 100),
('Teclado mecánico', 120.00, 50),
('Monitor 24 pulgadas', 780.50, 20),
('Docking station USB-C', 220.00, 15);
GO