USE [School_OlympiadDB];
GO

-- ============================================================
-- 0. Расширяем длину Email (если нужно)
-- ============================================================
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Классный_руководитель') AND name = 'Email' AND max_length < 100)
    ALTER TABLE [dbo].[Классный_руководитель] ALTER COLUMN [Email] NVARCHAR(100) NOT NULL;
GO

-- ============================================================
-- 1. Очистка таблиц
-- ============================================================
DELETE FROM [dbo].[Участие_в_олимпиаде];
DBCC CHECKIDENT (N'[dbo].[Участие_в_олимпиаде]', RESEED, 0);
DELETE FROM [dbo].[Школьник];
DBCC CHECKIDENT (N'[dbo].[Школьник]', RESEED, 0);
DELETE FROM [dbo].[Олимпиада];
DBCC CHECKIDENT (N'[dbo].[Олимпиада]', RESEED, 0);
DELETE FROM [dbo].[Класс];
DBCC CHECKIDENT (N'[dbo].[Класс]', RESEED, 0);
DELETE FROM [dbo].[Классный_руководитель];
DBCC CHECKIDENT (N'[dbo].[Классный_руководитель]', RESEED, 0);
DELETE FROM [dbo].[Образовательное_учереждение];
DBCC CHECKIDENT (N'[dbo].[Образовательное_учереждение]', RESEED, 0);
GO

-- ============================================================
-- 2. Образовательные учреждения
-- ============================================================
SET IDENTITY_INSERT [dbo].[Образовательное_учереждение] ON;
INSERT INTO [dbo].[Образовательное_учереждение] ([ID], [Наименование], [Адрес])
VALUES 
(1, N'Средняя школа №45',  N'г. Москва, ул. Ленина, 12            '),
(2, N'Гимназия №7',        N'г. Москва, пр-т Мира, 8              '),
(3, N'Лицей №1535',        N'г. Москва, ул. Усачёва, 50           ');
SET IDENTITY_INSERT [dbo].[Образовательное_учереждение] OFF;
GO

-- ============================================================
-- 3. Генерация классов и классных руководителей (исправлено)
-- ============================================================

-- Списки для классных руководителей
DECLARE @TeacherLNames TABLE (ID INT IDENTITY(1,1), Name NVARCHAR(20));
INSERT INTO @TeacherLNames (Name) VALUES 
(N'Иванова'),(N'Петрова'),(N'Сидорова'),(N'Кузнецова'),(N'Смирнова'),(N'Васильева'),(N'Попова'),(N'Соколова'),(N'Михайлова'),(N'Фёдорова'),
(N'Морозова'),(N'Волкова'),(N'Алексеева'),(N'Лебедева'),(N'Егорова'),(N'Павлова'),(N'Козлова'),(N'Степанова'),(N'Николаева'),(N'Орлова'),
(N'Андреева'),(N'Макарова'),(N'Крылова'),(N'Григорьева'),(N'Белова'),(N'Титова'),(N'Ларина'),(N'Быкова'),(N'Зайцева'),(N'Виноградова');

DECLARE @TeacherFNames TABLE (ID INT IDENTITY(1,1), Name NVARCHAR(20));
INSERT INTO @TeacherFNames (Name) VALUES 
(N'Мария'),(N'Елена'),(N'Ольга'),(N'Татьяна'),(N'Анна'),(N'Светлана'),(N'Наталья'),(N'Ирина'),(N'Людмила'),(N'Галина'),
(N'Александра'),(N'Валентина'),(N'Вера'),(N'Зоя'),(N'Надежда'),(N'Алла'),(N'Василиса'),(N'Анастасия'),(N'Полина'),(N'Евгения'),
(N'Дарья'),(N'Марина'),(N'Ксения'),(N'Алина'),(N'Диана'),(N'Валерия'),(N'Елизавета'),(N'Виктория'),(N'Юлия'),(N'Екатерина');

DECLARE @TeacherPatr TABLE (ID INT IDENTITY(1,1), Patronymic NVARCHAR(20));
INSERT INTO @TeacherPatr (Patronymic) VALUES 
(N'Петровна'),(N'Ивановна'),(N'Сергеевна'),(N'Алексеевна'),(N'Владимировна'),(N'Николаевна'),(N'Андреевна'),(N'Дмитриевна'),(N'Викторовна'),(N'Евгеньевна'),
(N'Борисовна'),(N'Григорьевна'),(N'Павловна'),(N'Семёновна'),(N'Васильевна'),(N'Михайловна'),(N'Леонидовна'),(N'Аркадьевна'),(N'Витальевна'),(N'Анатольевна'),
(N'Валентиновна'),(N'Геннадьевна'),(N'Константиновна'),(N'Максимовна'),(N'Олеговна'),(N'Романовна'),(N'Станиславовна'),(N'Юрьевна'),(N'Ярославовна'),(N'Фёдоровна');

-- Таблица букв
DECLARE @LettersTable TABLE (Idx INT, Letter CHAR(1));
INSERT INTO @LettersTable VALUES (1,'А'),(2,'Б'),(3,'В');

DECLARE @TeacherCounter INT = 1;
DECLARE @ClassNum INT, @LetterIdx INT, @Letter CHAR(1), @ClassName NVARCHAR(3);
DECLARE @TeacherSurname NVARCHAR(20), @TeacherName NVARCHAR(20), @TeacherPatrName NVARCHAR(20);
DECLARE @Phone NVARCHAR(11), @Email NVARCHAR(100), @TeacherID INT;

SET @ClassNum = 1;
WHILE @ClassNum <= 11
BEGIN
    SET @LetterIdx = 1;
    WHILE @LetterIdx <= 3
    BEGIN
        SELECT @Letter = Letter FROM @LettersTable WHERE Idx = @LetterIdx;
        IF NOT (@ClassNum = 11 AND @Letter = 'В')
        BEGIN
            -- Циклический перебор имён из списков
            SELECT @TeacherSurname = Name FROM @TeacherLNames WHERE ID = (@TeacherCounter % 30) + 1;
            SELECT @TeacherName = Name FROM @TeacherFNames WHERE ID = (@TeacherCounter % 30) + 1;
            SELECT @TeacherPatrName = Patronymic FROM @TeacherPatr WHERE ID = (@TeacherCounter % 30) + 1;
            
            SET @Phone = '7' + RIGHT('000000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000000 AS NVARCHAR(9)), 10);
            SET @Email = LOWER(@TeacherSurname + '.' + @TeacherName + '@school.ru');
            
            INSERT INTO [dbo].[Классный_руководитель] ([Фамилия], [Имя], [Отчество], [Телефон], [Email])
            VALUES (@TeacherSurname, @TeacherName, @TeacherPatrName, @Phone, @Email);
            SET @TeacherID = SCOPE_IDENTITY();
            
            SET @ClassName = CAST(@ClassNum AS NVARCHAR(2)) + @Letter;
            INSERT INTO [dbo].[Класс] ([Название], [Классный_руководитель_ID])
            VALUES (@ClassName, @TeacherID);
            
            SET @TeacherCounter = @TeacherCounter + 1;
        END
        SET @LetterIdx = @LetterIdx + 1;
    END
    SET @ClassNum = @ClassNum + 1;
END
GO

-- ============================================================
-- 4. Генерация школьников (20–30 на класс)
-- ============================================================

DECLARE @StudentLNames TABLE (ID INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO @StudentLNames (Name) VALUES 
(N'Иванов'),(N'Петров'),(N'Сидоров'),(N'Кузнецов'),(N'Смирнов'),(N'Васильев'),(N'Попов'),(N'Соколов'),(N'Михайлов'),(N'Фёдоров'),
(N'Морозов'),(N'Волков'),(N'Алексеев'),(N'Лебедев'),(N'Егоров'),(N'Павлов'),(N'Козлов'),(N'Степанов'),(N'Николаев'),(N'Орлов'),
(N'Андреев'),(N'Макаров'),(N'Крылов'),(N'Григорьев'),(N'Белов'),(N'Титов'),(N'Ларин'),(N'Быков'),(N'Зайцев'),(N'Виноградов'),
(N'Иванова'),(N'Петрова'),(N'Сидорова'),(N'Кузнецова'),(N'Смирнова'),(N'Васильева'),(N'Попова'),(N'Соколова'),(N'Михайлова'),(N'Фёдорова'),
(N'Морозова'),(N'Волкова'),(N'Алексеева'),(N'Лебедева'),(N'Егорова'),(N'Павлова'),(N'Козлова'),(N'Степанова'),(N'Николаева'),(N'Орлова'),
(N'Андреева'),(N'Макарова'),(N'Крылова'),(N'Григорьева'),(N'Белова'),(N'Титова'),(N'Ларина'),(N'Быкова'),(N'Зайцева'),(N'Виноградова');

DECLARE @StudentFNames TABLE (ID INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO @StudentFNames (Name) VALUES 
(N'Александр'),(N'Алексей'),(N'Андрей'),(N'Антон'),(N'Артём'),(N'Борис'),(N'Вадим'),(N'Валентин'),(N'Валерий'),(N'Василий'),
(N'Виктор'),(N'Виталий'),(N'Владимир'),(N'Вячеслав'),(N'Геннадий'),(N'Георгий'),(N'Григорий'),(N'Даниил'),(N'Денис'),(N'Дмитрий'),
(N'Евгений'),(N'Егор'),(N'Иван'),(N'Игорь'),(N'Илья'),(N'Кирилл'),(N'Константин'),(N'Лев'),(N'Леонид'),(N'Максим'),
(N'Матвей'),(N'Михаил'),(N'Никита'),(N'Николай'),(N'Олег'),(N'Павел'),(N'Пётр'),(N'Роман'),(N'Сергей'),(N'Станислав'),
(N'Степан'),(N'Тимур'),(N'Фёдор'),(N'Филипп'),(N'Юрий'),(N'Ярослав'),(N'Анастасия'),(N'Анна'),(N'Елена'),(N'Мария'),
(N'Ольга'),(N'Татьяна'),(N'Екатерина'),(N'Наталья'),(N'Ирина'),(N'Светлана'),(N'Юлия'),(N'Людмила'),(N'Галина'),(N'Вера');

DECLARE @StudentPatr TABLE (ID INT IDENTITY(1,1), Patronymic NVARCHAR(50));
INSERT INTO @StudentPatr (Patronymic) VALUES 
(N'Александрович'),(N'Алексеевич'),(N'Андреевич'),(N'Антонович'),(N'Артёмович'),(N'Борисович'),(N'Вадимович'),(N'Валентинович'),
(N'Валерьевич'),(N'Васильевич'),(N'Викторович'),(N'Витальевич'),(N'Владимирович'),(N'Вячеславович'),(N'Геннадьевич'),(N'Георгиевич'),
(N'Григорьевич'),(N'Даниилович'),(N'Денисович'),(N'Дмитриевич'),(N'Евгеньевич'),(N'Егорович'),(N'Иванович'),(N'Игоревич'),
(N'Ильич'),(N'Кириллович'),(N'Константинович'),(N'Львович'),(N'Леонидович'),(N'Максимович'),(N'Матвеевич'),(N'Михайлович'),
(N'Никитич'),(N'Николаевич'),(N'Олегович'),(N'Павлович'),(N'Петрович'),(N'Романович'),(N'Сергеевич'),(N'Станиславович'),
(N'Степанович'),(N'Тимурович'),(N'Фёдорович'),(N'Филиппович'),(N'Юрьевич'),(N'Ярославович'),(N'Александровна'),(N'Алексеевна'),
(N'Андреевна'),(N'Антоновна'),(N'Артёмовна'),(N'Борисовна'),(N'Вадимовна'),(N'Валентиновна'),(N'Валерьевна'),(N'Васильевна'),
(N'Викторовна'),(N'Витальевна'),(N'Владимировна'),(N'Вячеславовна'),(N'Геннадьевна'),(N'Георгиевна'),(N'Григорьевна'),(N'Данииловна'),
(N'Денисовна'),(N'Дмитриевна'),(N'Евгеньевна'),(N'Егоровна'),(N'Ивановна'),(N'Игоревна'),(N'Ильинична'),(N'Кирилловна'),
(N'Константиновна'),(N'Львовна'),(N'Леонидовна'),(N'Максимовна'),(N'Матвеевна'),(N'Михайловна'),(N'Никитична'),(N'Николаевна'),
(N'Олеговна'),(N'Павловна'),(N'Петровна'),(N'Романовна'),(N'Сергеевна'),(N'Станиславовна'),(N'Степановна'),(N'Тимуровна'),
(N'Фёдоровна'),(N'Филипповна'),(N'Юрьевна'),(N'Ярославовна');

DECLARE @ClassID INT, @StudentCount INT, @StudentIndex INT, @SchoolID INT;
DECLARE @LNameVar NVARCHAR(50), @FNameVar NVARCHAR(50), @PatrVar NVARCHAR(50);

DECLARE curClass CURSOR FOR SELECT ID FROM [dbo].[Класс];
OPEN curClass;
FETCH NEXT FROM curClass INTO @ClassID;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @StudentCount = 20 + ABS(CHECKSUM(NEWID())) % 11;
    SET @StudentIndex = 1;
    WHILE @StudentIndex <= @StudentCount
    BEGIN
        SET @LNameVar = (SELECT TOP 1 Name FROM @StudentLNames ORDER BY NEWID());
        SET @FNameVar = (SELECT TOP 1 Name FROM @StudentFNames ORDER BY NEWID());
        SET @PatrVar = (SELECT TOP 1 Patronymic FROM @StudentPatr ORDER BY NEWID());
        SET @SchoolID = 1 + ABS(CHECKSUM(NEWID())) % 3;
        
        INSERT INTO [dbo].[Школьник] ([Фамилия], [Имя], [Отчеcтво], [Образовательное_учереждение_ID], [Класс_ID])
        VALUES (@LNameVar, @FNameVar, @PatrVar, @SchoolID, @ClassID);
        
        SET @StudentIndex = @StudentIndex + 1;
    END
    FETCH NEXT FROM curClass INTO @ClassID;
END
CLOSE curClass;
DEALLOCATE curClass;
GO

-- ============================================================
-- 5. Генерация олимпиад
-- ============================================================
DECLARE @Subjects TABLE (SubjectName NVARCHAR(30));
INSERT INTO @Subjects VALUES 
(N'Математика'), (N'Русский язык'), (N'Физика'), (N'Химия'), (N'Биология'),
(N'История'), (N'Информатика'), (N'Английский язык'), (N'Литература'), (N'География');

DECLARE @Subj NVARCHAR(30), @TargetClass INT, @OlympiadDate DATE;
DECLARE curSubj CURSOR FOR SELECT SubjectName FROM @Subjects;
OPEN curSubj;
FETCH NEXT FROM curSubj INTO @Subj;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @CountOlimp INT = 3 + ABS(CHECKSUM(NEWID())) % 3;
    DECLARE @i INT = 1;
    WHILE @i <= @CountOlimp
    BEGIN
        SET @TargetClass = 1 + ABS(CHECKSUM(NEWID())) % 11;
        SET @OlympiadDate = DATEADD(day, -ABS(CHECKSUM(NEWID())) % 1000, GETDATE());
        INSERT INTO [dbo].[Олимпиада] ([Предмет], [Класс_за_который_выполняеться_задание], [Дата_проведения])
        VALUES (@Subj, @TargetClass, @OlympiadDate);
        SET @i = @i + 1;
    END
    FETCH NEXT FROM curSubj INTO @Subj;
END
CLOSE curSubj;
DEALLOCATE curSubj;
GO

-- ============================================================
-- 6. Генерация участий (1–3 на школьника)
-- ============================================================
DECLARE @StudentID INT, @ParticCount INT, @OlympiadID INT, @Score INT, @Result NVARCHAR(10);
DECLARE curStud CURSOR FOR SELECT ID FROM [dbo].[Школьник];
OPEN curStud;
FETCH NEXT FROM curStud INTO @StudentID;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ParticCount = 1 + ABS(CHECKSUM(NEWID())) % 3;
    DECLARE @j INT = 1;
    WHILE @j <= @ParticCount
    BEGIN
        SELECT TOP 1 @OlympiadID = ID FROM [dbo].[Олимпиада] ORDER BY NEWID();
        SET @Score = ABS(CHECKSUM(NEWID())) % 101;
        SET @Result = CASE 
            WHEN @Score >= 85 THEN N'победитель'
            WHEN @Score >= 70 THEN N'призёр'
            ELSE N'участник'
        END;
        INSERT INTO [dbo].[Участие_в_олимпиаде] 
            ([Количество_баллов], [Результат_участия], [Школьник_ID], [Олимпиада_ID])
        VALUES (@Score, @Result, @StudentID, @OlympiadID);
        SET @j = @j + 1;
    END
    FETCH NEXT FROM curStud INTO @StudentID;
END
CLOSE curStud;
DEALLOCATE curStud;
GO

-- Исправление названий классов
UPDATE [dbo].[Класс] SET [Название] = N'1А' WHERE [ID] = 1;
UPDATE [dbo].[Класс] SET [Название] = N'1Б' WHERE [ID] = 2;
UPDATE [dbo].[Класс] SET [Название] = N'1В' WHERE [ID] = 3;
UPDATE [dbo].[Класс] SET [Название] = N'2А' WHERE [ID] = 4;
UPDATE [dbo].[Класс] SET [Название] = N'2Б' WHERE [ID] = 5;
UPDATE [dbo].[Класс] SET [Название] = N'2В' WHERE [ID] = 6;
UPDATE [dbo].[Класс] SET [Название] = N'3А' WHERE [ID] = 7;
UPDATE [dbo].[Класс] SET [Название] = N'3Б' WHERE [ID] = 8;
UPDATE [dbo].[Класс] SET [Название] = N'3В' WHERE [ID] = 9;
UPDATE [dbo].[Класс] SET [Название] = N'4А' WHERE [ID] = 10;
UPDATE [dbo].[Класс] SET [Название] = N'4Б' WHERE [ID] = 11;
UPDATE [dbo].[Класс] SET [Название] = N'4В' WHERE [ID] = 12;
UPDATE [dbo].[Класс] SET [Название] = N'5А' WHERE [ID] = 13;
UPDATE [dbo].[Класс] SET [Название] = N'5Б' WHERE [ID] = 14;
UPDATE [dbo].[Класс] SET [Название] = N'5В' WHERE [ID] = 15;
UPDATE [dbo].[Класс] SET [Название] = N'6А' WHERE [ID] = 16;
UPDATE [dbo].[Класс] SET [Название] = N'6Б' WHERE [ID] = 17;
UPDATE [dbo].[Класс] SET [Название] = N'6В' WHERE [ID] = 18;
UPDATE [dbo].[Класс] SET [Название] = N'7А' WHERE [ID] = 19;
UPDATE [dbo].[Класс] SET [Название] = N'7Б' WHERE [ID] = 20;
UPDATE [dbo].[Класс] SET [Название] = N'7В' WHERE [ID] = 21;
UPDATE [dbo].[Класс] SET [Название] = N'8А' WHERE [ID] = 22;
UPDATE [dbo].[Класс] SET [Название] = N'8Б' WHERE [ID] = 23;
UPDATE [dbo].[Класс] SET [Название] = N'8В' WHERE [ID] = 24;
UPDATE [dbo].[Класс] SET [Название] = N'9А' WHERE [ID] = 25;
UPDATE [dbo].[Класс] SET [Название] = N'9Б' WHERE [ID] = 26;
UPDATE [dbo].[Класс] SET [Название] = N'9В' WHERE [ID] = 27;
UPDATE [dbo].[Класс] SET [Название] = N'10А' WHERE [ID] = 28;
UPDATE [dbo].[Класс] SET [Название] = N'10Б' WHERE [ID] = 29;
UPDATE [dbo].[Класс] SET [Название] = N'10В' WHERE [ID] = 30;
UPDATE [dbo].[Класс] SET [Название] = N'11А' WHERE [ID] = 31;
UPDATE [dbo].[Класс] SET [Название] = N'11Б' WHERE [ID] = 32;