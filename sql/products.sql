select p.producto_nombre as nombre, p.*, uhp.id_unidad, u.nombre_unidad, inventario.id_inventario,
		inventario.id_local, inventario.cantidad, inventario.fraccion, l.nombre_linea, m.nombre_marca, 
        f.nombre_familia, g.nombre_grupo, p2.proveedor_nombre, i.nombre_impuesto, uhprecio.id_precio
	from producto p
left join lineas l
	on l.id_linea = p.producto_linea
left join marcas m
	on m.id_marca = p.producto_marca
left join familia f
	on f.id_familia = p.producto_familia
left join grupos g
	on g.id_grupo = p.produto_grupo
left join proveedor p2
	on p2.id_proveedor = p.producto_proveedor
left join impuestos i
	on i.id_impuesto = p.producto_impuesto
left join (SELECT DISTINCT inventario.id_producto, inventario.id_inventario, 
							inventario.cantidad, inventario.fraccion, 
							inventario.id_local 
				FROM inventario 
			WHERE inventario.id_local=1
			 ORDER by id_inventario desc) as inventario
	on inventario.id_producto = p.producto_id
left join unidades_has_producto uhp
	on uhp.producto_id = p.producto_id and uhp.orden = 1
left join unidades u
	on u.id_unidad = uhp.id_unidad
left join unidades_has_precio uhprecio
	on uhprecio.id_producto = p.producto_id
where p.producto_estado in(0,1) and p.producto_estatus = 1
group by p.producto_id


