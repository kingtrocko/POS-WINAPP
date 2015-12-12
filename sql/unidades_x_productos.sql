
select * from inventario

select u.id_unidad, u.nombre_unidad, u.abreviatura, uhp.producto_id, 
		uhp.unidades, uhp.orden #, p.precio, id_precio
	from unidades u
inner join unidades_has_producto uhp
	on uhp.id_unidad = u.id_unidad
#inner join unidades_has_precio p
#	on p.id_unidad = uhp.id_unidad
where u.estatus_unidad = 1
order by uhp.producto_id, uhp.orden


select * from unidades_has_producto

select * from unidades_has_precio where id_unidad = 5

