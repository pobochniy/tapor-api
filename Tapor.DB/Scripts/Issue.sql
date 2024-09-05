CREATE TABLE `Issue` (
     `Id` bigint(20) NOT NULL AUTO_INCREMENT,
     `Assignee` char(36) CHARACTER SET ascii DEFAULT NULL COMMENT 'Исполнитель',
     `Reporter` char(36) CHARACTER SET ascii NOT NULL COMMENT 'Инициатор',
     `Summary` longtext,
     `Description` longtext,
     `Type` int(11) NOT NULL,
     `Status` int(11) NOT NULL,
     `Priority` int(11) NOT NULL,
     `Size` tinyint(3) unsigned NOT NULL,
     `EstimatedTime` decimal(65,30) DEFAULT NULL,
     `CreateDate` datetime(6) NOT NULL,
     `DueDate` date DEFAULT NULL,
     `EpicLink` int(11) DEFAULT NULL,
     PRIMARY KEY (`Id`),
     UNIQUE KEY `IX_Issue_Id` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4