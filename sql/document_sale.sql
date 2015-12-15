select 
	(
		case 
			when (`documento_venta`.`documento_Numero` = '9999999999') then
				convert( right(concat('0000',(ifnull(`documento_venta`.`documento_Serie`,0) + 1)), 4) using latin1)
			when (ifnull(`documento_venta`.`documento_Serie`,0) = 0) then convert( right(concat('0000', 1), 4) using latin1)
			else `documento_venta`.`documento_Serie` 
		end
	) AS `SERIE`,
	(
		case 
			when (`documento_venta`.`documento_Numero` = '9999999999') then 
				right(concat((`documento_venta`.`documento_Numero` + 2)),10)
			else 
				right(concat('0000000000',(`documento_venta`.`documento_Numero` + 1)),10) 
		end
	) AS `NUMERO`, 
    `documento_venta`.`nombre_tipo_documento` AS `Documento`
from `documento_venta` 
	where `documento_venta`.`nombre_tipo_documento` = 'NOTA DE PEDIDO'
    order by `documento_venta`.`documento_Serie`, `documento_venta`.`documento_Numero` desc limit 0,1