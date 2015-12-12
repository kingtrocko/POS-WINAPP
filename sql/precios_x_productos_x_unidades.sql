select * 
	from unidades_has_precio uhp2
inner join precios p
	on p.id_precio = uhp2.id_precio
where uhp2.id_producto = 1 and p.id_precio = 1
order by uhp2.id_unidad