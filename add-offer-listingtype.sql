-- add-offer-listingtype.sql
USE `db`;

LOCK TABLES `offers` WRITE;
/*!40000 ALTER TABLE `offers` DISABLE KEYS */;

-- ListingType: Angebot (0) oder Gesuch (1)
-- In C# Default: ListingType.Angebot (=0)
ALTER TABLE `offers`
  ADD COLUMN `ListingType` TINYINT NOT NULL DEFAULT 0
  AFTER `Status`;

-- Falls du sicherstellen willst, dass alte Daten sauber auf Angebot stehen:
UPDATE `offers` SET `ListingType` = 0 WHERE `ListingType` IS NULL;

/*!40000 ALTER TABLE `offers` ENABLE KEYS */;
UNLOCK TABLES;

-- Done: add-offer-listingtype.sql
