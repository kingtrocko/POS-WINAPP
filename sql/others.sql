# $existencia = get_by_id_row
SELECT * FROM inventario 
	WHERE id_producto=1 AND id_local=1 
ORDER by id_inventario DESC LIMIT 1

# $unidades = get_by_producto()
SELECT * 
	from unidades_has_producto uhp
join unidades u 
	on u.id_unidad = uhp.id_unidad
join producto p
	on p.producto_id = uhp.producto_id
where uhp.producto_id = 1
order by uhp.orden asc

# precios_cliente
select * 
	from cliente cli
join ciudades c
	on c.ciudad_id = cli.ciudad_id
join estados e
	on e.estados_id = c.estado_id
join pais p
	on p.id_pais = e.pais_id
where cli.id_cliente = 7

# precios_normal
select * 
	from precios p
where p.estatus_precio = 1

  
