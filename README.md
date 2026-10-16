# ClientesApi

## Motor BD - SQL Server
```sql
CREATE TABLE clientes
(
	id			INT IDENTITY(1,1) PRIMARY KEY,
	apellidos	NVARCHAR(70)	NOT NULL,
	nombres		NVARCHAR(70)	NOT NULL,
	telefono	NVARCHAR(9)		NOT NULL,
	correo		NVARCHAR(150)	NOT NULL,
	dni			CHAR(8)			NOT NULL,
	direccion	NVARCHAR(150)	NOT NULL,
	CONSTRAINT uk_dni_cli UNIQUE (dni),
	CONSTRAINT ck_dni_cli CHECK (dni LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
)
GO

CREATE TABLE productos
(
	id				INT IDENTITY(1,1) PRIMARY KEY,
	nombre			NVARCHAR(100)	NOT NULL, -- string
	descripcion		NVARCHAR(100)	NOT NULL, -- string
	precio			DECIMAL(7,2)	NOT NULL, -- double
	garantia		TINYINT			NOT NULL, -- int
	numserie		NVARCHAR(40)	NULL      -- string
)
GO
```