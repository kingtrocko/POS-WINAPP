/*
Pasos al guardar factura:
 - Reducir inventario
 
*/

select  p.producto_id, p.producto_nombre, m.nombre_marca, f.nombre_familia as categoria,
		pro.proveedor_nombre, p.producto_estatus, p.producto_costo_unitario, p.producto_impuesto,
        i.cantidad as cantidad_inventario, i.id_inventario
	from producto as p
left join inventario i
	on i.id_producto = p.producto_id
left join marcas m
	on m.id_marca = p.producto_marca
left join familia f
	on f.id_familia = p.producto_familia
left join proveedor pro
	on pro.id_proveedor = p.producto_proveedor
order by p.producto_nombre asc

