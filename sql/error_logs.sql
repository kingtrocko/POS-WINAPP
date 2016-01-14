CREATE TABLE `error_logs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `message` varchar(200) DEFAULT NULL,
  `stack_trace` varchar(200) DEFAULT NULL,
  `inner_exception` varchar(200) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
