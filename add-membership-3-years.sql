-- add-membership-3-years.sql
USE `db`;

LOCK TABLES `memberships` WRITE;
/*!40000 ALTER TABLE `memberships` DISABLE KEYS */;

INSERT INTO `memberships`
  (`MembershipID`, `Description`, `DurationDays`, `IsActive`, `Name`, `Price`)
VALUES
  (5, '3 Jahre Mitgliedschaft', 1095, 1, '3 Jahre', 79.000000000000000000000000000000);

/*!40000 ALTER TABLE `memberships` ENABLE KEYS */;
UNLOCK TABLES;
-- Done: add-membership-3-years.sql