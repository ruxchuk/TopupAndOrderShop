/*
SQLyog Ultimate v9.63 
MySQL - 5.1.50-community : Database - dang_takong_sale
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`dang_takong_sale` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `dang_takong_sale`;

/*Table structure for table `buy` */

DROP TABLE IF EXISTS `buy`;

CREATE TABLE `buy` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) unsigned NOT NULL DEFAULT '0',
  `date_buy` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `buy_product` */

DROP TABLE IF EXISTS `buy_product`;

CREATE TABLE `buy_product` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `product_id` int(11) unsigned NOT NULL DEFAULT '0',
  `price` double NOT NULL DEFAULT '0',
  `buy_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `customer` */

DROP TABLE IF EXISTS `customer`;

CREATE TABLE `customer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `phone_number_id` int(11) unsigned DEFAULT '0',
  `name` varchar(255) NOT NULL,
  `address` varchar(255) DEFAULT NULL,
  `date_add` datetime DEFAULT NULL,
  `date_update` datetime DEFAULT NULL,
  `deleted` tinyint(3) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=552 DEFAULT CHARSET=utf8;

/*Table structure for table `phone_number` */

DROP TABLE IF EXISTS `phone_number`;

CREATE TABLE `phone_number` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `phone_number` varchar(128) NOT NULL,
  `network` varchar(128) NOT NULL,
  `date_update` datetime DEFAULT NULL,
  `date_add` datetime DEFAULT NULL,
  `deleted` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8241 DEFAULT CHARSET=utf8;

/*Table structure for table `product` */

DROP TABLE IF EXISTS `product`;

CREATE TABLE `product` (
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_name` varchar(255) NOT NULL,
  `price` double NOT NULL DEFAULT '0',
  `date_update` datetime NOT NULL,
  `barcode` varchar(255) NOT NULL,
  `type_value` varchar(128) NOT NULL,
  `deleted` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=471 DEFAULT CHARSET=utf8;

/*Table structure for table `topup` */

DROP TABLE IF EXISTS `topup`;

CREATE TABLE `topup` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `phone_number_id` int(11) unsigned DEFAULT NULL,
  `date_add` datetime DEFAULT NULL,
  `topup_amount` int(11) DEFAULT NULL,
  `is_topup` tinyint(4) DEFAULT '0',
  `date_topup` datetime DEFAULT NULL,
  `deleted` int(11) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8;

/*Table structure for table `topup_behindhand` */

DROP TABLE IF EXISTS `topup_behindhand`;

CREATE TABLE `topup_behindhand` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `topup_id` int(11) unsigned NOT NULL DEFAULT '0',
  `customer_name` varchar(128) DEFAULT NULL,
  `price` varchar(128) DEFAULT NULL,
  `date_add` datetime DEFAULT NULL,
  `date_payment` datetime DEFAULT NULL,
  `paid` tinyint(3) NOT NULL DEFAULT '0',
  `deleted` int(11) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

/*Table structure for table `water_delivery` */

DROP TABLE IF EXISTS `water_delivery`;

CREATE TABLE `water_delivery` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) unsigned DEFAULT NULL,
  `amount` int(11) DEFAULT NULL,
  `paid` int(11) DEFAULT NULL,
  `detail` varchar(255) DEFAULT NULL,
  `date_add` datetime DEFAULT NULL,
  `date_complete` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1684 DEFAULT CHARSET=utf8;

/* Procedure structure for procedure `sp_get_list_customer` */

/*!50003 DROP PROCEDURE IF EXISTS  `sp_get_list_customer` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_list_customer`(in p_customer_id int, in p_customer_name varchar(255))
BEGIN
	#Routine body goes here...

SELECT
  a.*,
	b.phone_number,
	b.network
FROM `customer` a
LEFT JOIN `phone_number` b ON
(a.phone_number_id = b.id AND b.deleted = 0)
WHERE 1
AND a.deleted = 0
AND IF (p_customer_id != 0, a.id = p_customer_id, a.id > p_customer_id )
AND IF (p_customer_name != '', a.name LIKE CONCAT('%', p_customer_name, '%'), a.name != '')
LIMIT 100
;

END */$$
DELIMITER ;

/* Procedure structure for procedure `sp_get_list_phone_number` */

/*!50003 DROP PROCEDURE IF EXISTS  `sp_get_list_phone_number` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_list_phone_number`(in p_phone_number varchar(255))
BEGIN
	#Routine body goes here...

SELECT 
  a.`phone_number`,
  a.`network`,
  a.`date_update` AS a_date_update,
  a.`date_add`,
	a.id AS phone_number_id,
	IFNULL(b.id, 0) AS customer_id,
  b.`name`,
  b.`date_update` AS b_date_update,
  b.`date_add`,
  b.`address`
	
FROM 
	phone_number 	a
LEFT JOIN customer b
ON a.id = b.phone_number_id 
	AND b.deleted = 0
WHERE 1
	AND a.deleted = 0 
	AND IF(p_phone_number != '', 
	a.phone_number LIKE CONCAT('%', p_phone_number, '%'), 1)
	
GROUP BY a.phone_number 
ORDER BY a.date_update DESC
;
END */$$
DELIMITER ;

/* Procedure structure for procedure `sp_get_list_topup` */

/*!50003 DROP PROCEDURE IF EXISTS  `sp_get_list_topup` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_list_topup`(in p_is_topup int, in p_date_start varchar(128), in p_date_end varchar(128))
BEGIN
	#Routine body goes here...

SELECT 
  a.id AS topup_id,
	b.phone_number,
	a.topup_amount,
	b.network,
	IFNULL(c.`name`, 'ไม่มีชื่อ') AS customer_name,
	a.date_add,
	IFNULL(c.id, 0) AS customer_id,
	b.id AS phone_number_id
FROM
  `topup` a 
  INNER JOIN `phone_number` b 
    ON a.`phone_number_id` = b.`id` 
  LEFT JOIN `customer` c 
    ON c.`id` = a.`customer_id` 
    AND c.`deleted` = 0 
WHERE 1 
  AND a.`deleted` = 0 
  AND b.`deleted` = 0 
  AND a.`is_topup` = p_is_topup
	#AND a.date_add BETWEEN p_date_start 
	#AND p_date_end
	#AND a.date_add Bentaween
;


END */$$
DELIMITER ;

/* Procedure structure for procedure `sp_set_deleted_topup` */

/*!50003 DROP PROCEDURE IF EXISTS  `sp_set_deleted_topup` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_set_deleted_topup`(in p_topup_id int)
BEGIN
	#Routine body goes here...

UPDATE topup SET deleted = 1 WHERE id = p_topup_id;

END */$$
DELIMITER ;

/* Procedure structure for procedure `sp_set_is_topup_all` */

/*!50003 DROP PROCEDURE IF EXISTS  `sp_set_is_topup_all` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_set_is_topup_all`()
BEGIN
	#Routine body goes here...

UPDATE 
	topup 
SET 
	is_topup = 1, 
	date_topup = NOW()
WHERE 1
AND is_topup = 0 
AND deleted = 0;
END */$$
DELIMITER ;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
