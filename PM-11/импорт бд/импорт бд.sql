-- Создание базы данных с минимальными параметрами
CREATE DATABASE [School_OlympiadDB];
GO

USE [School_OlympiadDB];
GO

-- Таблица: Классный_руководитель
CREATE TABLE [dbo].[Классный_руководитель](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Фамилия] [nvarchar](20) NOT NULL,
	[Имя] [nvarchar](20) NOT NULL,
	[Отчество] [nvarchar](20) NULL,
	[Телефон] [nvarchar](11) NOT NULL,
	[Email] [nvarchar](20) NOT NULL,
	CONSTRAINT [PK_Классный руководитель] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Таблица: Класс
CREATE TABLE [dbo].[Класс](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Название] [nvarchar](3) NOT NULL,
	[Классный_руководитель_ID] [int] NOT NULL,
	CONSTRAINT [PK_Класс] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Таблица: Образовательное_учереждение
CREATE TABLE [dbo].[Образовательное_учереждение](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](max) NOT NULL,
	[Адрес] [nchar](100) NOT NULL,
	CONSTRAINT [PK_Образовательное_организация] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Таблица: Олимпиада
CREATE TABLE [dbo].[Олимпиада](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Предмет] [nvarchar](30) NOT NULL,
	[Класс_за_который_выполняеться_задание] [int] NOT NULL,
	[Дата_проведения] [date] NOT NULL,
	CONSTRAINT [PK_Олимпиада] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Таблица: Школьник
CREATE TABLE [dbo].[Школьник](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Фамилия] [nvarchar](50) NOT NULL,
	[Имя] [nvarchar](50) NOT NULL,
	[Отчеcтво] [nvarchar](50) NULL,
	[Образовательное_учереждение_ID] [int] NOT NULL,
	[Класс_ID] [int] NOT NULL,
	CONSTRAINT [PK_Школьник] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Таблица: Участие_в_олимпиаде
CREATE TABLE [dbo].[Участие_в_олимпиаде](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Количество_баллов] [int] NOT NULL,
	[Результат_участия] [nvarchar](10) NOT NULL,
	[Школьник_ID] [int] NOT NULL,
	[Олимпиада_ID] [int] NOT NULL,
	CONSTRAINT [PK_Участие_в_олимпиаде] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

-- Представление: Сведенье_о_Студентах
CREATE VIEW [dbo].[Сведенье_о_Студентах]
AS
SELECT 
    CONCAT(ш.Фамилия, ' ', ш.Имя, ISNULL(' ' + ш.Отчеcтво, '')) AS ФИО,
    кл.Название AS Класс,
    CONCAT(кр.Фамилия, ' ', кр.Имя, ISNULL(' ' + кр.Отчество, '')) AS [Классный руководитель],
    COUNT(у.ID) AS [Количество участий в олимпиаде]
FROM [dbo].[Школьник] ш
INNER JOIN [dbo].[Класс] кл ON ш.Класс_ID = кл.ID
INNER JOIN [dbo].[Классный_руководитель] кр ON кл.Классный_руководитель_ID = кр.ID
LEFT JOIN [dbo].[Участие_в_олимпиаде] у ON ш.ID = у.Школьник_ID
GROUP BY ш.ID, ш.Фамилия, ш.Имя, ш.Отчеcтво, кл.Название, кр.Фамилия, кр.Имя, кр.Отчество;
GO

-- Представление: Сведенье_о_Классном_руководителе
CREATE VIEW [dbo].[Сведенье_о_Классном_руководителе]
AS
SELECT 
    CONCAT(кр.Фамилия, ' ', кр.Имя, ISNULL(' ' + кр.Отчество, '')) AS [ФИО классного руководителя],
    кл.Название AS Класс,
    COUNT(DISTINCT ш.ID) AS [Количество школьников в классе],
    COUNT(у.ID) AS [Количество участий в олимпиаде от класса]
FROM [dbo].[Классный_руководитель] кр
INNER JOIN [dbo].[Класс] кл ON кр.ID = кл.Классный_руководитель_ID
INNER JOIN [dbo].[Школьник] ш ON кл.ID = ш.Класс_ID
LEFT JOIN [dbo].[Участие_в_олимпиаде] у ON ш.ID = у.Школьник_ID
GROUP BY кр.ID, кр.Фамилия, кр.Имя, кр.Отчество, кл.ID, кл.Название;
GO

-- Внешние ключи
ALTER TABLE [dbo].[Класс] ADD CONSTRAINT [FK_Класс_Классный_руководитель] FOREIGN KEY([Классный_руководитель_ID]) REFERENCES [dbo].[Классный_руководитель] ([ID]);
GO
ALTER TABLE [dbo].[Участие_в_олимпиаде] ADD CONSTRAINT [FK_Участие_в_олимпиаде_Олимпиада] FOREIGN KEY([Олимпиада_ID]) REFERENCES [dbo].[Олимпиада] ([ID]);
GO
ALTER TABLE [dbo].[Участие_в_олимпиаде] ADD CONSTRAINT [FK_Участие_в_олимпиаде_Школьник] FOREIGN KEY([Школьник_ID]) REFERENCES [dbo].[Школьник] ([ID]);
GO
ALTER TABLE [dbo].[Школьник] ADD CONSTRAINT [FK_Школьник_Класс] FOREIGN KEY([Класс_ID]) REFERENCES [dbo].[Класс] ([ID]);
GO
ALTER TABLE [dbo].[Школьник] ADD CONSTRAINT [FK_Школьник_Образовательное_учереждение] FOREIGN KEY([Образовательное_учереждение_ID]) REFERENCES [dbo].[Образовательное_учереждение] ([ID]);
GO

-- CHECK-ограничения
ALTER TABLE [dbo].[Классный_руководитель] ADD CONSTRAINT [CHK_Телефон] CHECK (([Телефон] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'));
GO
ALTER TABLE [dbo].[Участие_в_олимпиаде] ADD CONSTRAINT [CHK_Количество_баллов] CHECK (([Количество_баллов] >= 0 AND [Количество_баллов] <= 100));
GO
ALTER TABLE [dbo].[Участие_в_олимпиаде] ADD CONSTRAINT [CHK_Результат_участия] CHECK ((LOWER([Результат_участия]) = N'участник' OR LOWER([Результат_участия]) = N'победитель' OR LOWER([Результат_участия]) = N'призёр'));
GO