USE master;
GO


-- Eliminar base de datos si existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'BigFoodDB')
BEGIN
    ALTER DATABASE BigFoodDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BigFoodDB;
END
GO

-- Crear base de datos
CREATE DATABASE BigFoodDB;
GO

USE BigFoodDB;
GO


-- 1. TABLA: Usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    loginn NVARCHAR(50) NOT NULL UNIQUE,
    passwordd NVARCHAR(255) NOT NULL, 
    fechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    estado BIT NOT NULL DEFAULT 1, -- 1=Activo, 0=Inactivo
);
GO

-- 2. TABLA: Bitacora (Auditoría)
CREATE TABLE Bitacora (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Tabla NVARCHAR(50) NOT NULL,
    Usuario NVARCHAR(50) NOT NULL,
    Maquina NVARCHAR(100) NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    TipoMov NVARCHAR(20) NOT NULL, -- INSERT, UPDATE, DELETE
    Registro NVARCHAR(MAX) NULL -- JSON o texto con los datos del registro
);
GO

-- 3. TABLA: Clientes (datos desde API_Gometa)
CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    cedula_legal NVARCHAR(20) NOT NULL UNIQUE,
    tipoCedula NVARCHAR(10) NOT NULL, -- Física, Jurídica, DIMEX
    NombreCompleto NVARCHAR(150) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    fechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    estado BIT NOT NULL DEFAULT 1, -- 1=Activo, 0=Inactivo
    Usuario NVARCHAR(50) NOT NULL, -- Usuario que registró
);
GO

-- 4. TABLA: Productos
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CodigoInterno NVARCHAR(50) NOT NULL UNIQUE,
    CodigoBarras NVARCHAR(50) NULL UNIQUE,
    Descripcion NVARCHAR(200) NOT NULL,
    PrecioVenta DECIMAL(12,2) NOT NULL CHECK (PrecioVenta >= 0),
    Descuento DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (Descuento >= 0 AND Descuento <= 100),
    Impuesto DECIMAL(5,2) NOT NULL DEFAULT 13 CHECK (Impuesto >= 0 AND Impuesto <= 100),
    UnidadMedia NVARCHAR(20) NOT NULL DEFAULT 'Unidad',
    PrecioCompra DECIMAL(12,2) NOT NULL CHECK (PrecioCompra >= 0),
    Usuario NVARCHAR(50) NOT NULL, -- Usuario que creó/modificó
    Existencia INT NOT NULL DEFAULT 0 CHECK (Existencia >= 0),
);
GO

-- 5. TABLA: Facturas
CREATE TABLE Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL UNIQUE,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    codCliente INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    Subtotal DECIMAL(12,2) NOT NULL DEFAULT 0,
    MontoDescuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    MontoImpuesto DECIMAL(12,2) NOT NULL DEFAULT 0,
    Total DECIMAL(12,2) NOT NULL DEFAULT 0,
    estado NVARCHAR(15) NOT NULL DEFAULT 'Pagada' CHECK (estado IN ('Pagada', 'Pendiente', 'Anulada')),
    Usuario NVARCHAR(50) NOT NULL, -- Cajero que facturó
    TipoPago NVARCHAR(20) NOT NULL CHECK (TipoPago IN ('Efectivo', 'Tarjeta', 'Sinpe Movil')),
    Condicion NVARCHAR(10) NOT NULL CHECK (Condicion IN ('Contado', 'Credito')),
);
GO

-- 6. TABLA: Det_Facturas
CREATE TABLE Det_Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    numFacturas NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Facturas(numero),
    codInterno NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Productos(CodigoInterno),
    cantidad INT NOT NULL CHECK (cantidad > 0),
    PrecioUnitario DECIMAL(12,2) NOT NULL CHECK (PrecioUnitario >= 0),
    Subtotal DECIMAL(12,2) NOT NULL DEFAULT 0,
    PorDescuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    PorImp DECIMAL(12,2) NOT NULL DEFAULT 0,
);
GO

-- 7. TABLA: CuentasPorCobrar
CREATE TABLE CuentasPorCobrar (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumFactura NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Facturas(numero),
    CodCliente INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id),
    FechaFactura DATETIME NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    MontoFactura DECIMAL(12,2) NOT NULL,
    SaldoPendiente DECIMAL(12,2) NOT NULL,
    Usuario NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente', 'Pagada', 'Vencida')),
    FechaVencimiento DATE NOT NULL,
    FechaPago DATE NULL
);
GO

INSERT INTO Usuarios(loginn, passwordd, fechaRegistro, estado) values
('admin','admin123',GETDATE(),'1');

select * from Usuarios;