CREATE DATABASE SubscriptionServiceConfiguration;

USE SubscriptionServiceConfiguration;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `EndPoints` (
    `EndPointId` char(36) NOT NULL,
    `Name` longtext NULL,
    `Url` longtext NULL,
    CONSTRAINT `PK_EndPoints` PRIMARY KEY (`EndPointId`)
);

CREATE TABLE `SubscriptionStream` (
    `Id` char(36) NOT NULL,
    `IsNetCoreDomainStream` bit NOT NULL,
    `StreamName` longtext NULL,
    `SubscriptionType` int NOT NULL,
    CONSTRAINT `PK_SubscriptionStream` PRIMARY KEY (`Id`)
);

CREATE TABLE `CatchUpSubscriptions` (
    `Id` char(36) NOT NULL,
    `CreateDateTime` datetime(6) NOT NULL,
    `EndPointId` char(36) NOT NULL,
    `Name` longtext NULL,
    `Position` int NOT NULL,
    `StreamName` longtext NULL,
    CONSTRAINT `PK_CatchUpSubscriptions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CatchUpSubscriptions_EndPoints_EndPointId` FOREIGN KEY (`EndPointId`) REFERENCES `EndPoints` (`EndPointId`) ON DELETE CASCADE
);

CREATE TABLE `SubscriptionGroups` (
    `Id` char(36) NOT NULL,
    `BufferSize` int NULL,
    `EndPointId` char(36) NOT NULL,
    `Name` longtext NULL,
    `StreamPosition` int NULL,
    `SubscriptionStreamId` char(36) NOT NULL,
    CONSTRAINT `PK_SubscriptionGroups` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SubscriptionGroups_EndPoints_EndPointId` FOREIGN KEY (`EndPointId`) REFERENCES `EndPoints` (`EndPointId`) ON DELETE CASCADE,
    CONSTRAINT `FK_SubscriptionGroups_SubscriptionStream_SubscriptionStreamId` FOREIGN KEY (`SubscriptionStreamId`) REFERENCES `SubscriptionStream` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_CatchUpSubscriptions_EndPointId` ON `CatchUpSubscriptions` (`EndPointId`);

CREATE INDEX `IX_SubscriptionGroups_EndPointId` ON `SubscriptionGroups` (`EndPointId`);

CREATE INDEX `IX_SubscriptionGroups_SubscriptionStreamId` ON `SubscriptionGroups` (`SubscriptionStreamId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20181210110753_InitialDatabase', '2.1.4-rtm-31024');

ALTER TABLE `SubscriptionStream` DROP COLUMN `IsNetCoreDomainStream`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20181212111524_RemoveIsNetCoreStream', '2.1.4-rtm-31024');

