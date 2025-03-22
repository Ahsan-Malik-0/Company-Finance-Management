<-----------------  Create ---------------------->
--CREATE TABLE DailyRecords(
--	id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
--	netWorth INT,
--	differ INT,
--	date DATE,
--	month INT
--)
--ALTER TABLE DailyRecords
--ADD month INT;
--UPDATE DailyRecords
--    SET month = 2
--    WHERE date BETWEEN '2025-03-03' AND '2025-03-07';
--CREATE TABLE Spends(
--	id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
--	name VARCHAR(20),
--	item VARCHAR(50),
--	cost INT,
--	status VARCHAR(6),
--	dailyRecordId INT,
--	FOREIGN KEY (dailyRecordId) REFERENCES DailyRecords(id) ON DELETE CASCADE
--)
--CREATE TABLE Recives(
--	id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
--	name VARCHAR(50),
--	item VARCHAR(20),
--	cost INT,
--	dailyRecordId INT,
--	FOREIGN KEY (dailyRecordId) REFERENCES DailyRecords(id) ON DELETE CASCADE
--)
--ALTER TABLE Spends 
--ALTER COLUMN item VARCHAR(50);
--CREATE TABLE Others(
--	id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
--	name VARCHAR(20),
--	item VARCHAR(20),
--	cost INT,
--	dailyRecordId INT,
--	FOREIGN KEY (dailyRecordId) REFERENCES DailyRecords(id) ON DELETE CASCADE
--)
--CREATE TABLE Others(
--	id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
--	name VARCHAR(20),
--	cost INT,
--	dailyRecordId INT,
--	FOREIGN KEY (dailyRecordId) REFERENCES DailyRecords(id) ON DELETE CASCADE
--)
--------------------------------------------------------------------------------------------

<--------------- SELECT ------------------------>
SELECT * FROM DailyRecords
SELECT * FROM Spends
SELECT * FROM Spends WHERE status = 'Debit'
SELECT name AS [Name], cost AS [Cost], status AS [Credit] FROM Spends
SELECT CONCAT(cost,' - ',name) AS [Spend], status AS [Credit] FROM Spends
SELECT * FROM Recives
SELECT * FROM Others

SELECT SUM(cost) AS [Spend Total] FROM Spends WHERE status = 'Debit'
SELECT SUM(cost) AS [Recive Total] FROM Recives
SELECT SUM(cost) AS [Other Total] FROM Others


<---------------- DELETE OR DROP -------------------->
DELETE FROM Spends WHERE DailyRecordId = 5
DROP TABLE DailyRecords
DROP TABLE Spends
DROP TABLE Recives
DROP TABLE Others
DELETE FROM Spend WHERE DailyRecordId = 24

<------------------- MIAN TABLE ------------------->
SELECT DailyRecord.id, 
       CAST(DailyRecord.netWorth AS VARCHAR) + '/' + CAST(DailyRecord.differ AS VARCHAR) AS NetWorth,
       --CAST(Spend.cost AS VARCHAR) + '-' + Spend.name AS Spend,
       CAST(Recive.cost AS VARCHAR) + '-' + Recive.name AS Recive,
       CAST(Other.cost AS VARCHAR) + '-' + Other.name AS Other,
       FORMAT(DailyRecord.date, 'dd-MM-yyyy') AS Date
       FROM DailyRecord
       --JOIN Spend ON DailyRecord.id = Spend.DailyRecordId
       JOIN Recive ON DailyRecord.id = Recive.DailyRecordId
       JOIN Other ON DailyRecord.id = Other.DailyRecordId


SELECT FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS Date 
FROM DailyRecords 
WHERE id = 7;

SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost as [Cost]
		FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Spends
JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
WHERE Spends.status = 'Debit'
ORDER BY DailyRecords.date

SELECT Recives.name AS [Name], Recives.item AS [Item], Recives.cost as [Cost],
		FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Recives
JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
ORDER BY DailyRecords.date

SELECT Others.name AS [Name], Others.item AS [Item], Others.cost as [Cost],
		FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Others
JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
ORDER BY DailyRecords.date

SELECT Others.name AS [Name], Others.item AS [Item], Others.cost AS [Cost],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Others
JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
WHERE DailyRecords.date BETWEEN '2025-02-11' AND '2025-02-28'
ORDER BY DailyRecords.date;

// ------------------------- 1 Day ----------------------------------  //
SELECT * FROM DailyRecords WHERE DailyRecords.date = '2025-03-11'

SELECT Spends.name AS [Spend Name], Spends.item AS [Spend Item], Spends.cost AS [Spend Cost], Spends.status as [Status],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Spends
JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
WHERE DailyRecords.date = '2025-03-11'
ORDER BY DailyRecords.date;

SELECT Recives.name AS [Recive Name], Recives.item AS [Recive Item], Recives.cost AS [Recive Cost],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Recives
JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
WHERE DailyRecords.date = '2025-03-11'
ORDER BY DailyRecords.date;

SELECT Others.name AS [Other Name], Others.item AS [Other Item], Others.cost AS [Other Cost],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Others
JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
WHERE DailyRecords.date = '2025-03-11'
ORDER BY DailyRecords.date;


-- Get Second Last Row
SELECT TOP 1(netWorth)
FROM DailyRecords
ORDER BY id DESC 
OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY;

-- GET SECOND LAST WITH GIVEN ID
SELECT netWorth
FROM DailyRecords 
WHERE id = (SELECT MAX(id) FROM DailyRecords WHERE id < 40);

SELECT * FROM DailyRecords

SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Spends
JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
WHERE DailyRecords.date BETWEEN '2025-02-01' AND '2025-02-28'
ORDER BY DailyRecords.date;

SELECT Others.name AS [Other Name], Others.item AS [Other Item], Others.cost AS [Other Cost],
       FORMAT(DailyRecords.date, 'dd-MM-yyyy') AS [Date]
FROM Others
JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
WHERE Others.name = 'net'

-- GET MONTH NUMBER FROM LAST DATA ENTRY
SELECT TOP 1 month
FROM DailyRecords
ORDER BY id DESC;

SELECT SUM(Spends.cost) AS Cost
FROM Spends
--JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
where Spends.status = 'Credit'


SELECT *
FROM Recives
JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
where DailyRecords.month = 1
ORDER BY DailyRecords.date


SELECT *
FROM Recives
JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
where DailyRecords.month = 1
ORDER BY DailyRecords.date

SELECT * FROM Spends WHERE status = 'Credit' AND item LIKE 'plate%'
SELECT SUM(cost) AS [HARIS] FROM Spends WHERE status = 'Credit' AND item LIKE 'plate%'
SELECT SUM(cost) AS [Bhatti] FROM Spends WHERE status = 'Credit' AND (item LIKE 'Business Card%' OR item LIKE 'Printing')
SELECT SUM(cost) AS [Rashid] FROM Spends WHERE status = 'Credit' AND (item LIKE 'Flex%' OR item LIKE 'Standies')