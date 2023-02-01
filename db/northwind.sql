/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 50731
 Source Host           : localhost:3306
 Source Schema         : northwind

 Target Server Type    : MySQL
 Target Server Version : 50731
 File Encoding         : 65001

 Date: 18/01/2023 14:03:52
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for categories
-- ----------------------------
DROP TABLE IF EXISTS `categories`;
CREATE TABLE `categories`  (
  `CategoryID` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Description` mediumtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `Picture` longblob NULL,
  PRIMARY KEY (`CategoryID`) USING BTREE,
  INDEX `CategoryName`(`CategoryName`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for customercustomerdemo
-- ----------------------------
DROP TABLE IF EXISTS `customercustomerdemo`;
CREATE TABLE `customercustomerdemo`  (
  `CustomerID` varchar(5) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `CustomerTypeID` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`CustomerID`, `CustomerTypeID`) USING BTREE,
  INDEX `FK_CustomerCustomerDemo`(`CustomerTypeID`) USING BTREE,
  CONSTRAINT `FK_CustomerCustomerDemo` FOREIGN KEY (`CustomerTypeID`) REFERENCES `customerdemographics` (`CustomerTypeID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_CustomerCustomerDemo_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for customerdemographics
-- ----------------------------
DROP TABLE IF EXISTS `customerdemographics`;
CREATE TABLE `customerdemographics`  (
  `CustomerTypeID` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `CustomerDesc` mediumtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  PRIMARY KEY (`CustomerTypeID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS `customers`;
CREATE TABLE `customers`  (
  `CustomerID` varchar(5) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `CompanyName` varchar(40) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ContactName` varchar(30) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ContactTitle` varchar(30) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Address` varchar(60) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `City` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Region` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `PostalCode` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Country` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Phone` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Fax` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  PRIMARY KEY (`CustomerID`) USING BTREE,
  INDEX `City`(`City`) USING BTREE,
  INDEX `CompanyName`(`CompanyName`) USING BTREE,
  INDEX `PostalCode`(`PostalCode`) USING BTREE,
  INDEX `Region`(`Region`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employees
-- ----------------------------
DROP TABLE IF EXISTS `employees`;
CREATE TABLE `employees`  (
  `EmployeeID` int(11) NOT NULL AUTO_INCREMENT,
  `LastName` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `FirstName` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Title` varchar(30) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `TitleOfCourtesy` varchar(25) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `BirthDate` datetime NULL DEFAULT NULL,
  `HireDate` datetime NULL DEFAULT NULL,
  `Address` varchar(60) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `City` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Region` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `PostalCode` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Country` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `HomePhone` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Extension` varchar(4) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Photo` longblob NULL,
  `Notes` mediumtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ReportsTo` int(11) NULL DEFAULT NULL,
  `PhotoPath` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Salary` float NULL DEFAULT NULL,
  PRIMARY KEY (`EmployeeID`) USING BTREE,
  INDEX `LastName`(`LastName`) USING BTREE,
  INDEX `PostalCode`(`PostalCode`) USING BTREE,
  INDEX `FK_Employees_Employees`(`ReportsTo`) USING BTREE,
  CONSTRAINT `FK_Employees_Employees` FOREIGN KEY (`ReportsTo`) REFERENCES `employees` (`EmployeeID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employeeterritories
-- ----------------------------
DROP TABLE IF EXISTS `employeeterritories`;
CREATE TABLE `employeeterritories`  (
  `EmployeeID` int(11) NOT NULL,
  `TerritoryID` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`EmployeeID`, `TerritoryID`) USING BTREE,
  INDEX `FK_EmployeeTerritories_Territories`(`TerritoryID`) USING BTREE,
  CONSTRAINT `FK_EmployeeTerritories_Employees` FOREIGN KEY (`EmployeeID`) REFERENCES `employees` (`EmployeeID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_EmployeeTerritories_Territories` FOREIGN KEY (`TerritoryID`) REFERENCES `territories` (`TerritoryID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for order details
-- ----------------------------
DROP TABLE IF EXISTS `order details`;
CREATE TABLE `order details`  (
  `OrderID` int(11) NOT NULL,
  `ProductID` int(11) NOT NULL,
  `UnitPrice` decimal(10, 4) NOT NULL DEFAULT 0.0000,
  `Quantity` smallint(2) NOT NULL DEFAULT 1,
  `Discount` double(8, 0) NOT NULL DEFAULT 0,
  PRIMARY KEY (`OrderID`, `ProductID`) USING BTREE,
  INDEX `FK_Order_Details_Products`(`ProductID`) USING BTREE,
  CONSTRAINT `FK_Order_Details_Orders` FOREIGN KEY (`OrderID`) REFERENCES `orders` (`OrderID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Order_Details_Products` FOREIGN KEY (`ProductID`) REFERENCES `products` (`ProductID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS `orders`;
CREATE TABLE `orders`  (
  `OrderID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerID` varchar(5) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `EmployeeID` int(11) NULL DEFAULT NULL,
  `OrderDate` datetime NULL DEFAULT NULL,
  `RequiredDate` datetime NULL DEFAULT NULL,
  `ShippedDate` datetime NULL DEFAULT NULL,
  `ShipVia` int(11) NULL DEFAULT NULL,
  `Freight` decimal(10, 4) NULL DEFAULT 0.0000,
  `ShipName` varchar(40) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ShipAddress` varchar(60) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ShipCity` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ShipRegion` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ShipPostalCode` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ShipCountry` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  PRIMARY KEY (`OrderID`) USING BTREE,
  INDEX `OrderDate`(`OrderDate`) USING BTREE,
  INDEX `ShippedDate`(`ShippedDate`) USING BTREE,
  INDEX `ShipPostalCode`(`ShipPostalCode`) USING BTREE,
  INDEX `FK_Orders_Customers`(`CustomerID`) USING BTREE,
  INDEX `FK_Orders_Employees`(`EmployeeID`) USING BTREE,
  INDEX `FK_Orders_Shippers`(`ShipVia`) USING BTREE,
  CONSTRAINT `FK_Orders_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `customers` (`CustomerID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Orders_Employees` FOREIGN KEY (`EmployeeID`) REFERENCES `employees` (`EmployeeID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Orders_Shippers` FOREIGN KEY (`ShipVia`) REFERENCES `shippers` (`ShipperID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 11085 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for products
-- ----------------------------
DROP TABLE IF EXISTS `products`;
CREATE TABLE `products`  (
  `ProductID` int(11) NOT NULL AUTO_INCREMENT,
  `ProductName` varchar(40) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `SupplierID` int(11) NULL DEFAULT NULL,
  `CategoryID` int(11) NULL DEFAULT NULL,
  `QuantityPerUnit` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `UnitPrice` decimal(10, 4) NULL DEFAULT 0.0000,
  `UnitsInStock` smallint(2) NULL DEFAULT 0,
  `UnitsOnOrder` smallint(2) NULL DEFAULT 0,
  `ReorderLevel` smallint(2) NULL DEFAULT 0,
  `Discontinued` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ProductID`) USING BTREE,
  INDEX `ProductName`(`ProductName`) USING BTREE,
  INDEX `FK_Products_Categories`(`CategoryID`) USING BTREE,
  INDEX `FK_Products_Suppliers`(`SupplierID`) USING BTREE,
  CONSTRAINT `FK_Products_Categories` FOREIGN KEY (`CategoryID`) REFERENCES `categories` (`CategoryID`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Products_Suppliers` FOREIGN KEY (`SupplierID`) REFERENCES `suppliers` (`SupplierID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 78 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for region
-- ----------------------------
DROP TABLE IF EXISTS `region`;
CREATE TABLE `region`  (
  `RegionID` int(11) NOT NULL,
  `RegionDescription` varchar(50) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`RegionID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for shippers
-- ----------------------------
DROP TABLE IF EXISTS `shippers`;
CREATE TABLE `shippers`  (
  `ShipperID` int(11) NOT NULL AUTO_INCREMENT,
  `CompanyName` varchar(40) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `Phone` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  PRIMARY KEY (`ShipperID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for suppliers
-- ----------------------------
DROP TABLE IF EXISTS `suppliers`;
CREATE TABLE `suppliers`  (
  `SupplierID` int(11) NOT NULL AUTO_INCREMENT,
  `CompanyName` varchar(40) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `ContactName` varchar(30) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `ContactTitle` varchar(30) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Address` varchar(60) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `City` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Region` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `PostalCode` varchar(10) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Country` varchar(15) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Phone` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Fax` varchar(24) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `HomePage` mediumtext CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  PRIMARY KEY (`SupplierID`) USING BTREE,
  INDEX `CompanyName`(`CompanyName`) USING BTREE,
  INDEX `PostalCode`(`PostalCode`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 30 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for territories
-- ----------------------------
DROP TABLE IF EXISTS `territories`;
CREATE TABLE `territories`  (
  `TerritoryID` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `TerritoryDescription` varchar(50) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `RegionID` int(11) NOT NULL,
  PRIMARY KEY (`TerritoryID`) USING BTREE,
  INDEX `FK_Territories_Region`(`RegionID`) USING BTREE,
  CONSTRAINT `FK_Territories_Region` FOREIGN KEY (`RegionID`) REFERENCES `region` (`RegionID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- View structure for alphabetical list of products
-- ----------------------------
DROP VIEW IF EXISTS `alphabetical list of products`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `alphabetical list of products` AS select `products`.`ProductID` AS `ProductID`,`products`.`ProductName` AS `ProductName`,`products`.`SupplierID` AS `SupplierID`,`products`.`CategoryID` AS `CategoryID`,`products`.`QuantityPerUnit` AS `QuantityPerUnit`,`products`.`UnitPrice` AS `UnitPrice`,`products`.`UnitsInStock` AS `UnitsInStock`,`products`.`UnitsOnOrder` AS `UnitsOnOrder`,`products`.`ReorderLevel` AS `ReorderLevel`,`products`.`Discontinued` AS `Discontinued`,`categories`.`CategoryName` AS `CategoryName` from (`categories` join `products` on((`categories`.`CategoryID` = `products`.`CategoryID`))) where (`products`.`Discontinued` = 0);

-- ----------------------------
-- View structure for category sales for 1997
-- ----------------------------
DROP VIEW IF EXISTS `category sales for 1997`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `category sales for 1997` AS select `product sales for 1997`.`CategoryName` AS `CategoryName`,sum(`product sales for 1997`.`ProductSales`) AS `CategorySales` from `product sales for 1997` group by `product sales for 1997`.`CategoryName`;

-- ----------------------------
-- View structure for current product list
-- ----------------------------
DROP VIEW IF EXISTS `current product list`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `current product list` AS select `products`.`ProductID` AS `ProductID`,`products`.`ProductName` AS `ProductName` from `products` where (`products`.`Discontinued` = 0);

-- ----------------------------
-- View structure for customer and suppliers by city
-- ----------------------------
DROP VIEW IF EXISTS `customer and suppliers by city`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `customer and suppliers by city` AS select `customers`.`City` AS `City`,`customers`.`CompanyName` AS `CompanyName`,`customers`.`ContactName` AS `ContactName`,'Customers' AS `Relationship` from `customers` union select `suppliers`.`City` AS `City`,`suppliers`.`CompanyName` AS `CompanyName`,`suppliers`.`ContactName` AS `ContactName`,'Suppliers' AS `Suppliers` from `suppliers` order by `City`,`CompanyName`;

-- ----------------------------
-- View structure for invoices
-- ----------------------------
DROP VIEW IF EXISTS `invoices`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `invoices` AS select `orders`.`ShipName` AS `ShipName`,`orders`.`ShipAddress` AS `ShipAddress`,`orders`.`ShipCity` AS `ShipCity`,`orders`.`ShipRegion` AS `ShipRegion`,`orders`.`ShipPostalCode` AS `ShipPostalCode`,`orders`.`ShipCountry` AS `ShipCountry`,`orders`.`CustomerID` AS `CustomerID`,`customers`.`CompanyName` AS `CustomerName`,`customers`.`Address` AS `Address`,`customers`.`City` AS `City`,`customers`.`Region` AS `Region`,`customers`.`PostalCode` AS `PostalCode`,`customers`.`Country` AS `Country`,((`employees`.`FirstName` + ' ') + `employees`.`LastName`) AS `Salesperson`,`orders`.`OrderID` AS `OrderID`,`orders`.`OrderDate` AS `OrderDate`,`orders`.`RequiredDate` AS `RequiredDate`,`orders`.`ShippedDate` AS `ShippedDate`,`shippers`.`CompanyName` AS `ShipperName`,`order details`.`ProductID` AS `ProductID`,`products`.`ProductName` AS `ProductName`,`order details`.`UnitPrice` AS `UnitPrice`,`order details`.`Quantity` AS `Quantity`,`order details`.`Discount` AS `Discount`,((((`order details`.`UnitPrice` * `order details`.`Quantity`) * (1 - `order details`.`Discount`)) / 100) * 100) AS `ExtendedPrice`,`orders`.`Freight` AS `Freight` from (((((`customers` join `orders` on((`customers`.`CustomerID` = `orders`.`CustomerID`))) join `employees` on((`employees`.`EmployeeID` = `orders`.`EmployeeID`))) join `order details` on((`orders`.`OrderID` = `order details`.`OrderID`))) join `products` on((`products`.`ProductID` = `order details`.`ProductID`))) join `shippers` on((`shippers`.`ShipperID` = `orders`.`ShipVia`)));

-- ----------------------------
-- View structure for order details extended
-- ----------------------------
DROP VIEW IF EXISTS `order details extended`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `order details extended` AS select `order details`.`OrderID` AS `OrderID`,`order details`.`ProductID` AS `ProductID`,`products`.`ProductName` AS `ProductName`,`order details`.`UnitPrice` AS `UnitPrice`,`order details`.`Quantity` AS `Quantity`,`order details`.`Discount` AS `Discount`,((((`order details`.`UnitPrice` * `order details`.`Quantity`) * (1 - `order details`.`Discount`)) / 100) * 100) AS `ExtendedPrice` from (`products` join `order details` on((`products`.`ProductID` = `order details`.`ProductID`)));

-- ----------------------------
-- View structure for order subtotals
-- ----------------------------
DROP VIEW IF EXISTS `order subtotals`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `order subtotals` AS select `order details`.`OrderID` AS `OrderID`,sum(((((`order details`.`UnitPrice` * `order details`.`Quantity`) * (1 - `order details`.`Discount`)) / 100) * 100)) AS `Subtotal` from `order details` group by `order details`.`OrderID`;

-- ----------------------------
-- View structure for orders qry
-- ----------------------------
DROP VIEW IF EXISTS `orders qry`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `orders qry` AS select `orders`.`OrderID` AS `OrderID`,`orders`.`CustomerID` AS `CustomerID`,`orders`.`EmployeeID` AS `EmployeeID`,`orders`.`OrderDate` AS `OrderDate`,`orders`.`RequiredDate` AS `RequiredDate`,`orders`.`ShippedDate` AS `ShippedDate`,`orders`.`ShipVia` AS `ShipVia`,`orders`.`Freight` AS `Freight`,`orders`.`ShipName` AS `ShipName`,`orders`.`ShipAddress` AS `ShipAddress`,`orders`.`ShipCity` AS `ShipCity`,`orders`.`ShipRegion` AS `ShipRegion`,`orders`.`ShipPostalCode` AS `ShipPostalCode`,`orders`.`ShipCountry` AS `ShipCountry`,`customers`.`CompanyName` AS `CompanyName`,`customers`.`Address` AS `Address`,`customers`.`City` AS `City`,`customers`.`Region` AS `Region`,`customers`.`PostalCode` AS `PostalCode`,`customers`.`Country` AS `Country` from (`customers` join `orders` on((`customers`.`CustomerID` = `orders`.`CustomerID`)));

-- ----------------------------
-- View structure for product sales for 1997
-- ----------------------------
DROP VIEW IF EXISTS `product sales for 1997`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `product sales for 1997` AS select `categories`.`CategoryName` AS `CategoryName`,`products`.`ProductName` AS `ProductName`,sum(((((`order details`.`UnitPrice` * `order details`.`Quantity`) * (1 - `order details`.`Discount`)) / 100) * 100)) AS `ProductSales` from (((`categories` join `products` on((`categories`.`CategoryID` = `products`.`CategoryID`))) join `order details` on((`products`.`ProductID` = `order details`.`ProductID`))) join `orders` on((`orders`.`OrderID` = `order details`.`OrderID`))) where (`orders`.`ShippedDate` between '1997-01-01' and '1997-12-31') group by `categories`.`CategoryName`,`products`.`ProductName`;

-- ----------------------------
-- View structure for products above average price
-- ----------------------------
DROP VIEW IF EXISTS `products above average price`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `products above average price` AS select `products`.`ProductName` AS `ProductName`,`products`.`UnitPrice` AS `UnitPrice` from `products` where (`products`.`UnitPrice` > (select avg(`products`.`UnitPrice`) from `products`));

-- ----------------------------
-- View structure for products by category
-- ----------------------------
DROP VIEW IF EXISTS `products by category`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `products by category` AS select `categories`.`CategoryName` AS `CategoryName`,`products`.`ProductName` AS `ProductName`,`products`.`QuantityPerUnit` AS `QuantityPerUnit`,`products`.`UnitsInStock` AS `UnitsInStock`,`products`.`Discontinued` AS `Discontinued` from (`categories` join `products` on((`categories`.`CategoryID` = `products`.`CategoryID`))) where (`products`.`Discontinued` <> 1);

-- ----------------------------
-- View structure for quarterly orders
-- ----------------------------
DROP VIEW IF EXISTS `quarterly orders`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `quarterly orders` AS select distinct `customers`.`CustomerID` AS `CustomerID`,`customers`.`CompanyName` AS `CompanyName`,`customers`.`City` AS `City`,`customers`.`Country` AS `Country` from (`customers` join `orders` on((`customers`.`CustomerID` = `orders`.`CustomerID`))) where (`orders`.`OrderDate` between '1997-01-01' and '1997-12-31');

-- ----------------------------
-- View structure for sales by category
-- ----------------------------
DROP VIEW IF EXISTS `sales by category`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `sales by category` AS select `categories`.`CategoryID` AS `CategoryID`,`categories`.`CategoryName` AS `CategoryName`,`products`.`ProductName` AS `ProductName`,sum(`order details extended`.`ExtendedPrice`) AS `ProductSales` from (((`categories` join `products` on((`categories`.`CategoryID` = `products`.`CategoryID`))) join `order details extended` on((`products`.`ProductID` = `order details extended`.`ProductID`))) join `orders` on((`orders`.`OrderID` = `order details extended`.`OrderID`))) where (`orders`.`OrderDate` between '1997-01-01' and '1997-12-31') group by `categories`.`CategoryID`,`categories`.`CategoryName`,`products`.`ProductName`;

-- ----------------------------
-- View structure for sales totals by amount
-- ----------------------------
DROP VIEW IF EXISTS `sales totals by amount`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `sales totals by amount` AS select `order subtotals`.`Subtotal` AS `SaleAmount`,`orders`.`OrderID` AS `OrderID`,`customers`.`CompanyName` AS `CompanyName`,`orders`.`ShippedDate` AS `ShippedDate` from ((`customers` join `orders` on((`customers`.`CustomerID` = `orders`.`CustomerID`))) join `order subtotals` on((`orders`.`OrderID` = `order subtotals`.`OrderID`))) where ((`order subtotals`.`Subtotal` > 2500) and (`orders`.`ShippedDate` between '1997-01-01' and '1997-12-31'));

-- ----------------------------
-- View structure for summary of sales by quarter
-- ----------------------------
DROP VIEW IF EXISTS `summary of sales by quarter`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `summary of sales by quarter` AS select `orders`.`ShippedDate` AS `ShippedDate`,`orders`.`OrderID` AS `OrderID`,`order subtotals`.`Subtotal` AS `Subtotal` from (`orders` join `order subtotals` on((`orders`.`OrderID` = `order subtotals`.`OrderID`))) where (`orders`.`ShippedDate` is not null);

-- ----------------------------
-- View structure for summary of sales by year
-- ----------------------------
DROP VIEW IF EXISTS `summary of sales by year`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `summary of sales by year` AS select `orders`.`ShippedDate` AS `ShippedDate`,`orders`.`OrderID` AS `OrderID`,`order subtotals`.`Subtotal` AS `Subtotal` from (`orders` join `order subtotals` on((`orders`.`OrderID` = `order subtotals`.`OrderID`))) where (`orders`.`ShippedDate` is not null);

-- ----------------------------
-- Procedure structure for CustOrderHist
-- ----------------------------
DROP PROCEDURE IF EXISTS `CustOrderHist`;
delimiter ;;
CREATE PROCEDURE `CustOrderHist`(in AtCustomerID varchar(5))
BEGIN

SELECT ProductName,
    SUM(Quantity) as TOTAL
FROM Products P,
     `Order Details` OD,
     Orders O,
     Customers C
WHERE C.CustomerID = AtCustomerID
  AND C.CustomerID = O.CustomerID
  AND O.OrderID = OD.OrderID
  AND OD.ProductID = P.ProductID
GROUP BY ProductName;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for CustOrdersOrders
-- ----------------------------
DROP PROCEDURE IF EXISTS `CustOrdersOrders`;
delimiter ;;
CREATE PROCEDURE `CustOrdersOrders`(in AtCustomerID varchar(5))
BEGIN
      SELECT OrderID,
	OrderDate,
	RequiredDate,
	ShippedDate
FROM Orders
WHERE CustomerID = AtCustomerID
ORDER BY OrderID;

END
;;
delimiter ;

-- ----------------------------
-- Function structure for DateOnly
-- ----------------------------
DROP FUNCTION IF EXISTS `DateOnly`;
delimiter ;;
CREATE FUNCTION `DateOnly`(InDateTime datetime)
 RETURNS varchar(10) CHARSET latin1
BEGIN

  DECLARE MyOutput varchar(10);
	SET MyOutput = DATE_FORMAT(InDateTime,'%Y-%m-%d');

  RETURN MyOutput;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Employee Sales by Country
-- ----------------------------
DROP PROCEDURE IF EXISTS `Employee Sales by Country`;
delimiter ;;
CREATE PROCEDURE `Employee Sales by Country`(in AtBeginning_Date Datetime,in AtEnding_Date Datetime)
BEGIN
  SELECT Employees.Country,
         Employees.LastName,
         Employees.FirstName,
            Orders.ShippedDate,
            Orders.OrderID,
 `Order Subtotals`.Subtotal AS SaleAmount
FROM Employees
 JOIN Orders ON Employees.EmployeeID = Orders.EmployeeID
      JOIN `Order Subtotals` ON Orders.OrderID = `Order Subtotals`.OrderID
WHERE Orders.ShippedDate Between AtBeginning_Date And AtEnding_Date;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for LookByFName
-- ----------------------------
DROP PROCEDURE IF EXISTS `LookByFName`;
delimiter ;;
CREATE PROCEDURE `LookByFName`(IN AtFirstLetter CHAR(1))
BEGIN
     SELECT * FROM Employees  Where LEFT(FirstName, 1)=AtFirstLetter;

END
;;
delimiter ;

-- ----------------------------
-- Function structure for MyRound
-- ----------------------------
DROP FUNCTION IF EXISTS `MyRound`;
delimiter ;;
CREATE FUNCTION `MyRound`(Operand DOUBLE,Places INTEGER)
 RETURNS double
  DETERMINISTIC
BEGIN

DECLARE x DOUBLE;
DECLARE i INTEGER;
DECLARE ix DOUBLE;

  SET x = Operand*POW(10,Places);
  SET i=x;
  
  IF (i-x) >= 0.5 THEN                   
    SET ix = 1;                  
  ELSE
    SET ix = 0;                 
  END IF;     

  SET x=i+ix;
  SET x=x/POW(10,Places);

RETURN x;


END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Sales by Year
-- ----------------------------
DROP PROCEDURE IF EXISTS `Sales by Year`;
delimiter ;;
CREATE PROCEDURE `Sales by Year`(in AtBeginning_Date Datetime,in AtEnding_Date Datetime)
BEGIN

    SELECT Orders.ShippedDate,
	   Orders.OrderID,
	  `Order Subtotals`.Subtotal,
	  ShippedDate AS Year
FROM Orders  JOIN `Order Subtotals` ON Orders.OrderID = `Order Subtotals`.OrderID
WHERE Orders.ShippedDate Between AtBeginning_Date And AtEnding_Date;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for SalesByCategory
-- ----------------------------
DROP PROCEDURE IF EXISTS `SalesByCategory`;
delimiter ;;
CREATE PROCEDURE `SalesByCategory`()
BEGIN

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_employees_cursor
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_employees_cursor`;
delimiter ;;
CREATE PROCEDURE `sp_employees_cursor`(IN city_in VARCHAR(15))
BEGIN
  DECLARE name_val VARCHAR(10);
  DECLARE surname_val VARCHAR(10);
  DECLARE photopath_val VARCHAR(255);

  DECLARE no_more_rows BOOLEAN;

  DECLARE fetch_status INT DEFAULT 0;

  DECLARE employees_cur CURSOR FOR SELECT firstname, lastname,photopath FROM employees WHERE city = city_in;

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET no_more_rows = TRUE;

  DROP TABLE IF EXISTS atpeople;
  CREATE TABLE atpeople(
    FirstName VARCHAR(10),
    LastName VARCHAR(20),
    PhotoPath VARCHAR(255)
  );


  OPEN employees_cur;
  select FOUND_ROWS() into fetch_status;


  the_loop: LOOP

    FETCH  employees_cur  INTO   name_val,surname_val,photopath_val;


    IF no_more_rows THEN
       CLOSE employees_cur;
       LEAVE the_loop;
    END IF;


    INSERT INTO atpeople SELECT  name_val,surname_val,photopath_val;

  END LOOP the_loop;

  SELECT * FROM atpeople;
  DROP TABLE atpeople;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_Employees_Insert
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_Employees_Insert`;
delimiter ;;
CREATE PROCEDURE `sp_Employees_Insert`(In AtLastName VARCHAR(20),
In AtFirstName VARCHAR(10),
In AtTitle VARCHAR(30),
In AtTitleOfCourtesy VARCHAR(25),
In AtBirthDate DateTime,
In AtHireDate DateTime,
In AtAddress VARCHAR(60),
In AtCity VARCHAR(15),
In AtRegion VARCHAR(15),
In AtPostalCode VARCHAR(10),
In AtCountry VARCHAR(15),
In AtHomePhone VARCHAR(24),
In AtExtension VARCHAR(4),
In AtPhoto LONGBLOB,
In AtNotes MEDIUMTEXT,
In AtReportsTo INTEGER,
IN AtPhotoPath VARCHAR(255),
OUT AtReturnID INTEGER)
BEGIN
Insert Into Employees Values(AtLastName,AtFirstName,AtTitle,AtTitleOfCourtesy,AtBirthDate,AtHireDate,AtAddress,AtCity,AtRegion,AtPostalCode,AtCountry,AtHomePhone,AtExtension,AtPhoto,AtNotes,AtReportsTo,AtPhotoPath);

	SELECT AtReturnID = LAST_INSERT_ID();
	
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_employees_rownum
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_employees_rownum`;
delimiter ;;
CREATE PROCEDURE `sp_employees_rownum`()
BEGIN

SELECT *
FROM
(select @rownum:=@rownum+1  as RowNum,
  p.* from employees p
   ,(SELECT @rownum:=0) R
   order by firstname desc limit 10
) a
WHERE a.RowNum >= 2 AND a.RowNum<= 4;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_Employees_SelectAll
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_Employees_SelectAll`;
delimiter ;;
CREATE PROCEDURE `sp_Employees_SelectAll`()
BEGIN
SELECT * FROM Employees;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_Employees_SelectRow
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_Employees_SelectRow`;
delimiter ;;
CREATE PROCEDURE `sp_Employees_SelectRow`(In AtEmployeeID INTEGER)
BEGIN
SELECT * FROM Employees Where EmployeeID = AtEmployeeID;

END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for sp_Employees_Update
-- ----------------------------
DROP PROCEDURE IF EXISTS `sp_Employees_Update`;
delimiter ;;
CREATE PROCEDURE `sp_Employees_Update`(In AtEmployeeID INTEGER,
In AtLastName VARCHAR(20),
In AtFirstName VARCHAR(10),
In AtTitle VARCHAR(30),
In AtTitleOfCourtesy VARCHAR(25),
In AtBirthDate DateTime,
In AtHireDate DateTime,
In AtAddress VARCHAR(60),
In AtCity VARCHAR(15),
In AtRegion VARCHAR(15),
In AtPostalCode VARCHAR(10),
In AtCountry VARCHAR(15),
In AtHomePhone VARCHAR(24),
In AtExtension VARCHAR(4),
In AtPhoto LONGBLOB,
In AtNotes MEDIUMTEXT,
In AtReportsTo INTEGER,
IN AtPhotoPath VARCHAR(255))
BEGIN
Update Employees
	Set
		LastName = AtLastName,
		FirstName = AtFirstName,
		Title = AtTitle,
		TitleOfCourtesy = AtTitleOfCourtesy,
		BirthDate = AtBirthDate,
		HireDate = AtHireDate,
		Address = AtAddress,
		City = AtCity,
		Region = AtRegion,
		PostalCode = AtPostalCode,
		Country = AtCountry,
		HomePhone = AtHomePhone,
		Extension = AtExtension,
		Photo = AtPhoto,
		Notes = AtNotes,
		ReportsTo = AtReportsTo,
    PhotoPath = AtPhotoPath
	Where
		EmployeeID = AtEmployeeID;

END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
