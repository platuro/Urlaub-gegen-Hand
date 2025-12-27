-- fix-utf8.sql
USE `db`;

-- Session charset
SET NAMES utf8mb4;
SET character_set_client = utf8mb4;
SET character_set_connection = utf8mb4;
SET character_set_results = utf8mb4;

-- DB defaults
ALTER DATABASE `db`
  CHARACTER SET = utf8mb4
  COLLATE = utf8mb4_0900_ai_ci;

-- Konvertiere Tabellen (kann je nach Datenmenge kurz dauern)
SET foreign_key_checks = 0;

ALTER TABLE `skills`
  CONVERT TO CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

ALTER TABLE `accomodations`
  CONVERT TO CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

SET foreign_key_checks = 1;

START TRANSACTION;

-- Fix nur da, wo es kaputt aussieht
UPDATE `skills`
SET `SkillDescrition` =
    CONVERT(CAST(CONVERT(`SkillDescrition` USING latin1) AS BINARY) USING utf8mb4)
WHERE `SkillDescrition` IS NOT NULL
  AND (
    `SkillDescrition` LIKE '%Ã%' OR
    `SkillDescrition` LIKE '%Â%' OR
    `SkillDescrition` LIKE '%â%'
  );

UPDATE `accomodations`
SET `NameAccommodationType` =
    CONVERT(CAST(CONVERT(`NameAccommodationType` USING latin1) AS BINARY) USING utf8mb4)
WHERE `NameAccommodationType` IS NOT NULL
  AND (
    `NameAccommodationType` LIKE '%Ã%' OR
    `NameAccommodationType` LIKE '%Â%' OR
    `NameAccommodationType` LIKE '%â%'
  );

COMMIT;

-- Checks
SELECT Skill_ID, SkillDescrition
FROM `skills`
WHERE `SkillDescrition` LIKE '%Ã%' OR `SkillDescrition` LIKE '%Â%' OR `SkillDescrition` LIKE '%â%'
ORDER BY Skill_ID;

SELECT Id, NameAccommodationType
FROM `accomodations`
WHERE `NameAccommodationType` LIKE '%Ã%' OR `NameAccommodationType` LIKE '%Â%' OR `NameAccommodationType` LIKE '%â%'
ORDER BY Id;
