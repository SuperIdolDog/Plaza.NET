/*
 Navicat Premium Dump SQL

 Source Server         : .
 Source Server Type    : MySQL
 Source Server Version : 80039 (8.0.39)
 Source Host           : localhost:3306
 Source Schema         : plazashop

 Target Server Type    : MySQL
 Target Server Version : 80039 (8.0.39)
 File Encoding         : 65001

 Date: 06/09/2025 14:09:38
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for __efmigrationshistory
-- ----------------------------
DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE `__efmigrationshistory`  (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for address
-- ----------------------------
DROP TABLE IF EXISTS `address`;
CREATE TABLE `address`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Province` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `City` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `County` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Detail` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Label` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDefault` tinyint(1) NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Address_IsDefault`(`IsDefault` ASC) USING BTREE,
  INDEX `IX_Address_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_Address_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5164 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for device
-- ----------------------------
DROP TABLE IF EXISTS `device`;
CREATE TABLE `device`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Location` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DeviceStatusItemId` int NOT NULL,
  `LastMaintenanceDate` datetime(6) NULL DEFAULT NULL,
  `DeviceTypeId` int NOT NULL,
  `FloorId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Device_DeviceStatusItemId`(`DeviceStatusItemId` ASC) USING BTREE,
  INDEX `IX_Device_DeviceTypeId`(`DeviceTypeId` ASC) USING BTREE,
  INDEX `IX_Device_FloorId`(`FloorId` ASC) USING BTREE,
  CONSTRAINT `FK_Device_DeviceType_DeviceTypeId` FOREIGN KEY (`DeviceTypeId`) REFERENCES `devicetype` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Device_DictionaryItem_DeviceStatusItemId` FOREIGN KEY (`DeviceStatusItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Device_Floor_FloorId` FOREIGN KEY (`FloorId`) REFERENCES `floor` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 32879 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for devicedata
-- ----------------------------
DROP TABLE IF EXISTS `devicedata`;
CREATE TABLE `devicedata`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Value` double NOT NULL,
  `DeviceDataUnitItemId` int NOT NULL,
  `DeviceId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_DeviceData_DeviceDataUnitItemId`(`DeviceDataUnitItemId` ASC) USING BTREE,
  INDEX `IX_DeviceData_DeviceId`(`DeviceId` ASC) USING BTREE,
  CONSTRAINT `FK_DeviceData_Device_DeviceId` FOREIGN KEY (`DeviceId`) REFERENCES `device` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_DeviceData_DictionaryItem_DeviceDataUnitItemId` FOREIGN KEY (`DeviceDataUnitItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 23024 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for devicetype
-- ----------------------------
DROP TABLE IF EXISTS `devicetype`;
CREATE TABLE `devicetype`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Manufacturer` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for dictionary
-- ----------------------------
DROP TABLE IF EXISTS `dictionary`;
CREATE TABLE `dictionary`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `SortOrder` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for dictionaryitem
-- ----------------------------
DROP TABLE IF EXISTS `dictionaryitem`;
CREATE TABLE `dictionaryitem`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Label` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `SortOrder` int NOT NULL,
  `IsDefault` tinyint(1) NOT NULL,
  `DictionaryId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_DictionaryItem_DictionaryId`(`DictionaryId` ASC) USING BTREE,
  INDEX `IX_DictionaryItem_Value`(`Value` ASC) USING BTREE,
  CONSTRAINT `FK_DictionaryItem_Dictionary_DictionaryId` FOREIGN KEY (`DictionaryId`) REFERENCES `dictionary` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 83 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employee
-- ----------------------------
DROP TABLE IF EXISTS `employee`;
CREATE TABLE `employee`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gender` int NOT NULL,
  `Wage` decimal(65, 30) NOT NULL,
  `CommissionRate` double NOT NULL,
  `Contact` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HireDate` datetime(6) NOT NULL,
  `EmployeeRoleId` int NOT NULL,
  `StoreId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Employee_EmployeeRoleId`(`EmployeeRoleId` ASC) USING BTREE,
  INDEX `IX_Employee_StoreId`(`StoreId` ASC) USING BTREE,
  CONSTRAINT `FK_Employee_EmployeeRole_EmployeeRoleId` FOREIGN KEY (`EmployeeRoleId`) REFERENCES `employeerole` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Employee_Store_StoreId` FOREIGN KEY (`StoreId`) REFERENCES `store` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 681016 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employeerole
-- ----------------------------
DROP TABLE IF EXISTS `employeerole`;
CREATE TABLE `employeerole`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StoreId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_EmployeeRole_StoreId`(`StoreId` ASC) USING BTREE,
  CONSTRAINT `FK_EmployeeRole_Store_StoreId` FOREIGN KEY (`StoreId`) REFERENCES `store` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 851357 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for floor
-- ----------------------------
DROP TABLE IF EXISTS `floor`;
CREATE TABLE `floor`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PlazaId` int NOT NULL,
  `FloorItemId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Floor_FloorItemId`(`FloorItemId` ASC) USING BTREE,
  INDEX `IX_Floor_PlazaId`(`PlazaId` ASC) USING BTREE,
  CONSTRAINT `FK_Floor_DictionaryItem_FloorItemId` FOREIGN KEY (`FloorItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Floor_Plaza_PlazaId` FOREIGN KEY (`PlazaId`) REFERENCES `plaza` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4694 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for loginlog
-- ----------------------------
DROP TABLE IF EXISTS `loginlog`;
CREATE TABLE `loginlog`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `LoginTime` datetime(6) NOT NULL,
  `LogoutTime` datetime(6) NULL DEFAULT NULL,
  `IPAddress` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DeviceInfo` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `FailureReason` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `UserId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_LoginLog_IPAddress`(`IPAddress` ASC) USING BTREE,
  INDEX `IX_LoginLog_LoginTime`(`LoginTime` ASC) USING BTREE,
  INDEX `IX_LoginLog_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_LoginLog_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for menupermission
-- ----------------------------
DROP TABLE IF EXISTS `menupermission`;
CREATE TABLE `menupermission`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MenuId` int NOT NULL,
  `PermissionId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_MenuPermission_Menu_Permission`(`MenuId` ASC, `PermissionId` ASC) USING BTREE,
  INDEX `IX_MenuPermission_MenuId`(`MenuId` ASC) USING BTREE,
  INDEX `IX_MenuPermission_PermissionId`(`PermissionId` ASC) USING BTREE,
  CONSTRAINT `FK_MenuPermission_Permission_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permission` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_MenuPermission_SysMenu_MenuId` FOREIGN KEY (`MenuId`) REFERENCES `sysmenu` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for notification
-- ----------------------------
DROP TABLE IF EXISTS `notification`;
CREATE TABLE `notification`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Content` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NotificationTypeItemId` int NOT NULL,
  `IsRead` tinyint(1) NOT NULL,
  `Link` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` int NOT NULL,
  `PublisherId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Notification_CreateTime`(`CreateTime` ASC) USING BTREE,
  INDEX `IX_Notification_IsRead`(`IsRead` ASC) USING BTREE,
  INDEX `IX_Notification_NotificationTypeItemId`(`NotificationTypeItemId` ASC) USING BTREE,
  INDEX `IX_Notification_PublisherId`(`PublisherId` ASC) USING BTREE,
  INDEX `IX_Notification_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_Notification_DictionaryItem_NotificationTypeItemId` FOREIGN KEY (`NotificationTypeItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Notification_User_PublisherId` FOREIGN KEY (`PublisherId`) REFERENCES `user` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Notification_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for operationlog
-- ----------------------------
DROP TABLE IF EXISTS `operationlog`;
CREATE TABLE `operationlog`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OperationTime` datetime(6) NOT NULL,
  `OperationType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Module` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Target` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Details` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IPAddress` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `ExecutionTime` int NOT NULL,
  `UserId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_OperationLog_Module`(`Module` ASC) USING BTREE,
  INDEX `IX_OperationLog_OperationTime`(`OperationTime` ASC) USING BTREE,
  INDEX `IX_OperationLog_OperationType`(`OperationType` ASC) USING BTREE,
  INDEX `IX_OperationLog_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_OperationLog_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for order
-- ----------------------------
DROP TABLE IF EXISTS `order`;
CREATE TABLE `order`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TotalAmount` decimal(18, 2) NOT NULL,
  `DeliveryType` int NOT NULL,
  `OrderStatuItemId` int NOT NULL,
  `ShippingAddress` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PickUpDate` datetime(6) NULL DEFAULT NULL,
  `StoreId` int NOT NULL,
  `CustomerId` int NOT NULL,
  `EmployeeId` int NULL DEFAULT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Order_CustomerId`(`CustomerId` ASC) USING BTREE,
  INDEX `IX_Order_EmployeeId`(`EmployeeId` ASC) USING BTREE,
  INDEX `IX_Order_OrderStatuItemId`(`OrderStatuItemId` ASC) USING BTREE,
  INDEX `IX_Order_StoreId`(`StoreId` ASC) USING BTREE,
  CONSTRAINT `FK_Order_DictionaryItem_OrderStatuItemId` FOREIGN KEY (`OrderStatuItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Order_Employee_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `employee` (`Id`) ON DELETE SET NULL ON UPDATE RESTRICT,
  CONSTRAINT `FK_Order_Store_StoreId` FOREIGN KEY (`StoreId`) REFERENCES `store` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Order_User_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `user` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 699671 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for orderitem
-- ----------------------------
DROP TABLE IF EXISTS `orderitem`;
CREATE TABLE `orderitem`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Quantity` int NOT NULL,
  `UnitPrice` decimal(65, 30) NOT NULL,
  `OrderId` int NOT NULL,
  `ProductSkuId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_OrderItem_OrderId`(`OrderId` ASC) USING BTREE,
  INDEX `IX_OrderItem_ProductId`(`ProductSkuId` ASC) USING BTREE,
  CONSTRAINT `FK_OrderItem_Order_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `order` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_OrderItem_ProductSku_ProductSkuId` FOREIGN KEY (`ProductSkuId`) REFERENCES `productsku` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 358442 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for parkingarea
-- ----------------------------
DROP TABLE IF EXISTS `parkingarea`;
CREATE TABLE `parkingarea`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FloorId` int NOT NULL,
  `ParkingAreaItemId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ParkingArea_FloorId`(`FloorId` ASC) USING BTREE,
  INDEX `IX_ParkingArea_ParkingAreaItemId`(`ParkingAreaItemId` ASC) USING BTREE,
  CONSTRAINT `FK_ParkingArea_DictionaryItem_ParkingAreaItemId` FOREIGN KEY (`ParkingAreaItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_ParkingArea_Floor_FloorId` FOREIGN KEY (`FloorId`) REFERENCES `floor` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2750 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for parkingrecord
-- ----------------------------
DROP TABLE IF EXISTS `parkingrecord`;
CREATE TABLE `parkingrecord`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PlateNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PlateImageUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EntryTime` datetime(6) NOT NULL,
  `ExitTime` datetime(6) NULL DEFAULT NULL,
  `ParkingFee` decimal(65, 30) NOT NULL,
  `IsPaid` tinyint(1) NOT NULL,
  `DeviceId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ParkingRecord_DeviceId`(`DeviceId` ASC) USING BTREE,
  INDEX `IX_ParkingRecord_EntryTime`(`EntryTime` ASC) USING BTREE,
  INDEX `IX_ParkingRecord_PlateNumber`(`PlateNumber` ASC) USING BTREE,
  CONSTRAINT `FK_ParkingRecord_Device_DeviceId` FOREIGN KEY (`DeviceId`) REFERENCES `device` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for parkingspot
-- ----------------------------
DROP TABLE IF EXISTS `parkingspot`;
CREATE TABLE `parkingspot`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ParkingSpotItemId` int NOT NULL,
  `ParkingAreaId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ParkingSpot_ParkingAreaId`(`ParkingAreaId` ASC) USING BTREE,
  INDEX `IX_ParkingSpot_ParkingSpotItemId`(`ParkingSpotItemId` ASC) USING BTREE,
  CONSTRAINT `FK_ParkingSpot_DictionaryItem_ParkingSpotItemId` FOREIGN KEY (`ParkingSpotItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_ParkingSpot_ParkingArea_ParkingAreaId` FOREIGN KEY (`ParkingAreaId`) REFERENCES `parkingarea` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 137451 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for paymentrecord
-- ----------------------------
DROP TABLE IF EXISTS `paymentrecord`;
CREATE TABLE `paymentrecord`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PaymentMethodItemId` int NOT NULL,
  `Amount` decimal(18, 2) NOT NULL,
  `PaymentTime` datetime(6) NOT NULL,
  `PaystatuItemId` int NOT NULL,
  `TransactionId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrderId` int NOT NULL,
  `UserId` int NOT NULL,
  `OrderEntityId` int NULL DEFAULT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_PaymentRecord_OrderEntityId`(`OrderEntityId` ASC) USING BTREE,
  INDEX `IX_PaymentRecord_OrderId`(`OrderId` ASC) USING BTREE,
  INDEX `IX_PaymentRecord_PaymentMethodItemId`(`PaymentMethodItemId` ASC) USING BTREE,
  INDEX `IX_PaymentRecord_PaystatuItemId`(`PaystatuItemId` ASC) USING BTREE,
  INDEX `IX_PaymentRecord_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_PaymentRecord_DictionaryItem_PaymentMethodItemId` FOREIGN KEY (`PaymentMethodItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_PaymentRecord_DictionaryItem_PaystatuItemId` FOREIGN KEY (`PaystatuItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_PaymentRecord_Order_OrderEntityId` FOREIGN KEY (`OrderEntityId`) REFERENCES `order` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_PaymentRecord_Order_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `order` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_PaymentRecord_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 322629 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for permission
-- ----------------------------
DROP TABLE IF EXISTS `permission`;
CREATE TABLE `permission`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Permission_Name`(`Name` ASC) USING BTREE,
  INDEX `IX_Permission_RoleId`(`RoleId` ASC) USING BTREE,
  CONSTRAINT `FK_Permission_UserRole_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `userrole` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for plaza
-- ----------------------------
DROP TABLE IF EXISTS `plaza`;
CREATE TABLE `plaza`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Latitude` double NOT NULL,
  `Longitude` double NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 667 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for product
-- ----------------------------
DROP TABLE IF EXISTS `product`;
CREATE TABLE `product`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductTypeId` int NOT NULL,
  `StoreId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Product_ProductTypeId`(`ProductTypeId` ASC) USING BTREE,
  INDEX `IX_Product_StoreId`(`StoreId` ASC) USING BTREE,
  CONSTRAINT `FK_Product_ProductType_ProductTypeId` FOREIGN KEY (`ProductTypeId`) REFERENCES `producttype` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Product_Store_StoreId` FOREIGN KEY (`StoreId`) REFERENCES `store` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 170415 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for productsku
-- ----------------------------
DROP TABLE IF EXISTS `productsku`;
CREATE TABLE `productsku`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SkuCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `BarCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ImageUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Price` decimal(18, 2) NOT NULL,
  `CostPrice` decimal(18, 2) NOT NULL,
  `MarketPrice` decimal(18, 2) NOT NULL,
  `StockQuantity` decimal(18, 2) NOT NULL,
  `Sales` int NOT NULL,
  `Weight` decimal(18, 2) NOT NULL,
  `IsEnabled` tinyint(1) NOT NULL,
  `ProductId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_ProductSku_SkuCode`(`SkuCode` ASC) USING BTREE,
  INDEX `IX_ProductSku_BarCode`(`BarCode` ASC) USING BTREE,
  INDEX `IX_ProductSku_ProductId`(`ProductId` ASC) USING BTREE,
  CONSTRAINT `FK_ProductSku_Product_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `product` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 136267 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for productskuspecvalue
-- ----------------------------
DROP TABLE IF EXISTS `productskuspecvalue`;
CREATE TABLE `productskuspecvalue`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProductSkuId` int NOT NULL,
  `ProductSpecValueId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_ProductSkuSpecValue_Sku_SpecValue`(`ProductSkuId` ASC, `ProductSpecValueId` ASC) USING BTREE,
  INDEX `IX_ProductSkuSpecValue_ProductSkuId`(`ProductSkuId` ASC) USING BTREE,
  INDEX `IX_ProductSkuSpecValue_ProductSpecValueId`(`ProductSpecValueId` ASC) USING BTREE,
  CONSTRAINT `FK_ProductSkuSpecValue_ProductSku_ProductSkuId` FOREIGN KEY (`ProductSkuId`) REFERENCES `productsku` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_ProductSkuSpecValue_ProductSpecValue_ProductSpecValueId` FOREIGN KEY (`ProductSpecValueId`) REFERENCES `productspecvalue` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 196188 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for productspec
-- ----------------------------
DROP TABLE IF EXISTS `productspec`;
CREATE TABLE `productspec`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Sort` int NOT NULL,
  `GoodsId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ProductSpec_ProductId`(`GoodsId` ASC) USING BTREE,
  INDEX `IX_ProductSpec_Sort`(`Sort` ASC) USING BTREE,
  CONSTRAINT `FK_ProductSpec_Product_GoodsId` FOREIGN KEY (`GoodsId`) REFERENCES `product` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 511243 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for productspecvalue
-- ----------------------------
DROP TABLE IF EXISTS `productspecvalue`;
CREATE TABLE `productspecvalue`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SpecId` int NOT NULL,
  `Value` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ProductSpecValue_SpecId`(`SpecId` ASC) USING BTREE,
  INDEX `IX_ProductSpecValue_Value`(`Value` ASC) USING BTREE,
  CONSTRAINT `FK_ProductSpecValue_ProductSpec_SpecId` FOREIGN KEY (`SpecId`) REFERENCES `productspec` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 409047 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for producttype
-- ----------------------------
DROP TABLE IF EXISTS `producttype`;
CREATE TABLE `producttype`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StoreId` int NOT NULL,
  `ProductTypeUnitItemId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_ProductType_ProductTypeUnitItemId`(`ProductTypeUnitItemId` ASC) USING BTREE,
  INDEX `IX_ProductType_StoreId`(`StoreId` ASC) USING BTREE,
  CONSTRAINT `FK_ProductType_DictionaryItem_ProductTypeUnitItemId` FOREIGN KEY (`ProductTypeUnitItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_ProductType_Store_StoreId` FOREIGN KEY (`StoreId`) REFERENCES `store` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 212840 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for review
-- ----------------------------
DROP TABLE IF EXISTS `review`;
CREATE TABLE `review`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ReviewRatingItemId` int NOT NULL,
  `Content` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrderItemId` int NOT NULL,
  `UserId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Review_OrderItemId`(`OrderItemId` ASC) USING BTREE,
  INDEX `IX_Review_ReviewRatingItemId`(`ReviewRatingItemId` ASC) USING BTREE,
  INDEX `IX_Review_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_Review_DictionaryItem_ReviewRatingItemId` FOREIGN KEY (`ReviewRatingItemId`) REFERENCES `dictionaryitem` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_Review_OrderItem_OrderItemId` FOREIGN KEY (`OrderItemId`) REFERENCES `orderitem` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Review_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 32792 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for roleclaim
-- ----------------------------
DROP TABLE IF EXISTS `roleclaim`;
CREATE TABLE `roleclaim`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_RoleClaim_RoleId`(`RoleId` ASC) USING BTREE,
  CONSTRAINT `FK_RoleClaim_UserRole_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `userrole` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for shoppingcartitem
-- ----------------------------
DROP TABLE IF EXISTS `shoppingcartitem`;
CREATE TABLE `shoppingcartitem`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Quantity` int NOT NULL DEFAULT 1,
  `Selected` tinyint(1) NOT NULL,
  `UserId` int NOT NULL,
  `ProductSkuId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_ShoppingCartItem_User_Product`(`UserId` ASC, `ProductSkuId` ASC) USING BTREE,
  INDEX `IX_ShoppingCartItem_ProductSkuId`(`ProductSkuId` ASC) USING BTREE,
  INDEX `IX_ShoppingCartItem_Selected`(`Selected` ASC) USING BTREE,
  INDEX `IX_ShoppingCartItem_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_ShoppingCartItem_ProductSku_ProductSkuId` FOREIGN KEY (`ProductSkuId`) REFERENCES `productsku` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_ShoppingCartItem_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 299718 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for store
-- ----------------------------
DROP TABLE IF EXISTS `store`;
CREATE TABLE `store`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SwiperImageUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Location` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `BusinessHours` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Contact` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Rating` double NOT NULL,
  `IsOperating` tinyint(1) NOT NULL,
  `StoreTypeId` int NOT NULL,
  `FloorId` int NOT NULL,
  `UserId` int NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Store_FloorId`(`FloorId` ASC) USING BTREE,
  INDEX `IX_Store_StoreTypeId`(`StoreTypeId` ASC) USING BTREE,
  INDEX `IX_Store_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_Store_Floor_FloorId` FOREIGN KEY (`FloorId`) REFERENCES `floor` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_Store_StoreType_StoreTypeId` FOREIGN KEY (`StoreTypeId`) REFERENCES `storetype` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_Store_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 212840 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for storetype
-- ----------------------------
DROP TABLE IF EXISTS `storetype`;
CREATE TABLE `storetype`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sysmenu
-- ----------------------------
DROP TABLE IF EXISTS `sysmenu`;
CREATE TABLE `sysmenu`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Icon` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Url` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Order` int NOT NULL,
  `ParentId` int NULL DEFAULT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_SysMenu_Order`(`Order` ASC) USING BTREE,
  INDEX `IX_SysMenu_ParentId`(`ParentId` ASC) USING BTREE,
  INDEX `IX_SysMenu_Type`(`Type` ASC) USING BTREE,
  CONSTRAINT `FK_SysMenu_SysMenu_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `sysmenu` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `AvatarUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FullName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IDNumber` varchar(18) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `RegisterDate` datetime(6) NOT NULL,
  `isOnline` int NOT NULL,
  `LastLoginDate` datetime(6) NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `UserRoleId` int NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) NULL DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `UserNameIndex`(`NormalizedUserName` ASC) USING BTREE,
  INDEX `EmailIndex`(`NormalizedEmail` ASC) USING BTREE,
  INDEX `IX_User_IsDeleted`(`IsDeleted` ASC) USING BTREE,
  INDEX `IX_User_LastLoginDate`(`LastLoginDate` ASC) USING BTREE,
  INDEX `IX_User_RegisterDate`(`RegisterDate` ASC) USING BTREE,
  INDEX `IX_User_UserRoleId`(`UserRoleId` ASC) USING BTREE,
  CONSTRAINT `FK_User_UserRole_UserRoleId` FOREIGN KEY (`UserRoleId`) REFERENCES `userrole` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for userclaim
-- ----------------------------
DROP TABLE IF EXISTS `userclaim`;
CREATE TABLE `userclaim`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_UserClaim_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_UserClaim_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for userfeedback
-- ----------------------------
DROP TABLE IF EXISTS `userfeedback`;
CREATE TABLE `userfeedback`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Imageurl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Content` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Contact` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ReplyContent` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ReplyTime` datetime(6) NULL DEFAULT NULL,
  `UserId` int NOT NULL,
  `RepliedById` int NULL DEFAULT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `CreateTime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdateTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_UserFeedback_CreateTime`(`CreateTime` ASC) USING BTREE,
  INDEX `IX_UserFeedback_RepliedById`(`RepliedById` ASC) USING BTREE,
  INDEX `IX_UserFeedback_ReplyTime`(`ReplyTime` ASC) USING BTREE,
  INDEX `IX_UserFeedback_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_UserFeedback_User_RepliedById` FOREIGN KEY (`RepliedById`) REFERENCES `user` (`Id`) ON DELETE SET NULL ON UPDATE RESTRICT,
  CONSTRAINT `FK_UserFeedback_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for userlogin
-- ----------------------------
DROP TABLE IF EXISTS `userlogin`;
CREATE TABLE `userlogin`  (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`) USING BTREE,
  INDEX `IX_UserLogin_UserId`(`UserId` ASC) USING BTREE,
  CONSTRAINT `FK_UserLogin_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for userrole
-- ----------------------------
DROP TABLE IF EXISTS `userrole`;
CREATE TABLE `userrole`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_UserRole_Code`(`Code` ASC) USING BTREE,
  UNIQUE INDEX `RoleNameIndex`(`NormalizedName` ASC) USING BTREE,
  INDEX `IX_UserRole_IsDeleted`(`IsDeleted` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for userrolemapping
-- ----------------------------
DROP TABLE IF EXISTS `userrolemapping`;
CREATE TABLE `userrolemapping`  (
  `UserId` int NOT NULL,
  `RoleId` int NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`) USING BTREE,
  INDEX `IX_UserRoleMapping_RoleId`(`RoleId` ASC) USING BTREE,
  CONSTRAINT `FK_UserRoleMapping_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_UserRoleMapping_UserRole_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `userrole` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for usertoken
-- ----------------------------
DROP TABLE IF EXISTS `usertoken`;
CREATE TABLE `usertoken`  (
  `UserId` int NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`) USING BTREE,
  CONSTRAINT `FK_UserToken_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
