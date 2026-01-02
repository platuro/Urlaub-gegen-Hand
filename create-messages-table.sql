-- create-messages-table.sql
USE `db`;

SET NAMES utf8mb4;
SET character_set_client = utf8mb4;
SET character_set_connection = utf8mb4;
SET character_set_results = utf8mb4;

-- Erstelle Tabelle mit korrekter Collation
CREATE TABLE IF NOT EXISTS `messages` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `SenderId` CHAR(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `ReceiverId` CHAR(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `Subject` VARCHAR(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `Content` VARCHAR(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `SentAt` DATETIME(6) NOT NULL,
    `ReadAt` DATETIME(6) NULL,
    `IsRead` TINYINT(1) NOT NULL DEFAULT 0,
    `RelatedOfferId` INT NULL,
    
    PRIMARY KEY (`Id`),
    
    INDEX `IX_messages_SenderId` (`SenderId`),
    INDEX `IX_messages_ReceiverId` (`ReceiverId`),
    INDEX `IX_messages_RelatedOfferId` (`RelatedOfferId`),
    INDEX `IX_messages_SentAt` (`SentAt`),
    INDEX `IX_messages_IsRead` (`IsRead`),
    
    CONSTRAINT `FK_messages_users_SenderId` 
        FOREIGN KEY (`SenderId`) 
        REFERENCES `users` (`User_Id`) 
        ON DELETE CASCADE,
    
    CONSTRAINT `FK_messages_users_ReceiverId` 
        FOREIGN KEY (`ReceiverId`) 
        REFERENCES `users` (`User_Id`) 
        ON DELETE CASCADE,
    
    CONSTRAINT `FK_messages_offers_RelatedOfferId` 
        FOREIGN KEY (`RelatedOfferId`) 
        REFERENCES `offers` (`Id`) 
        ON DELETE SET NULL
        
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_unicode_ci;

DESCRIBE `messages`;
SHOW INDEX FROM `messages`;