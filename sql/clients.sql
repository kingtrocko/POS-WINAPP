        
select * 
	from cliente cli
join ciudades c
	on c.ciudad_id = cli.ciudad_id
join estados e
	on e.estados_id = c.estado_id
join pais p
	on p.id_pais = e.pais_id
join grupos_cliente gc 
	on gc.id_grupos_cliente = cli.grupo_id
left join precios
	on precios.id_precio = cli.categoria_precio
where cli.cliente_status = 1