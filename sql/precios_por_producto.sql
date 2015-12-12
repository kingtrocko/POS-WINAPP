select *
	from unidades_has_precio uhprecio
join producto p
	on uhprecio.id_producto = p.producto_id
left join impuestos i
	on i.id_impuesto = p.producto_impuesto
join precios
	on uhprecio.id_precio = precios.id_precio
join unidades u
	on u.id_unidad = uhprecio.id_unidad
join unidades_has_producto uhproducto
	on uhprecio.id_unidad = uhproducto.id_unidad and uhprecio.id_producto = uhproducto.producto_id
where uhprecio.id_precio = 1 and uhprecio.id_producto = 1
order by uhproducto.orden asc