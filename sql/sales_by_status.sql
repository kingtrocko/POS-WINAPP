select * 
	from venta v
join cliente c
	on c.id_cliente = v.id_cliente
join local l
	on l.int_local_id = v.local_id
join condiciones_pago cp
	on cp.id_condiciones = v.condicion_pago
join documento_venta dv
	on dv.id_tipo_documento = v.numero_documento
join usuario u
	on u.nUsuCodigo = v.id_vendedor
where v.local_id = 1 and v.venta_status = 1
order by v.venta_id desc

