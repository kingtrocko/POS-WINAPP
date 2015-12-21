set @rank = 0;
select @rank := @rank +1 as item_number,
 v.venta_id,v.total as montoTotal,v.subtotal as subTotal,
          	v.total_impuesto as impuesto, v.pagado, v.vuelto, pd.producto_nombre,pd.producto_cualidad, 
              pd.producto_id as producto_id, tr.cantidad as cantidad,tr.precio, pd.producto_costo_unitario,
          	tr.detalle_importe as importe, v.fecha as fechaemision, cre.int_credito_nrocuota, cre.dec_credito_montocuota,
          	p.nombre as vendedor,t.nombre_tipo_documento as descripcion, t.documento_Serie as serie, t.documento_Numero as numero, t.nombre_tipo_documento,
          	c.razon_social as cliente, c.id_cliente as cliente_id, c.direccion as direccion_cliente,
          	c.identificacion as documento_cliente, cp.id_condiciones, cp.nombre_condiciones, v.venta_status, u.id_unidad, u.nombre_unidad as unidad_medida, u.abreviatura,
          	i.porcentaje_impuesto, up.unidades, up.orden,
          	 (select config_value from configuraciones where config_key='EMPRESA_NOMBRE') as RazonSocialEmpresa,
          	 (select config_value from configuraciones where config_key='EMPRESA_DIRECCION') as DireccionEmpresa,
          	 (select config_value from configuraciones where config_key='EMPRESA_TELEFONO') as TelefonoEmpresa,
          	 (select abreviatura from unidades_has_producto join unidades on unidades.id_unidad=unidades_has_producto.id_unidad
          		where id_producto=pd.producto_id order by orden desc limit 1) as unidad_minima
          from venta as v
          	inner join usuario p on p.nUsuCodigo = v.id_vendedor
          	inner join documento_venta t on t.id_tipo_documento = v.numero_documento
          	inner join detalle_venta tr on tr.id_venta = v.venta_id
          	inner join cliente c on c.id_cliente = v.id_cliente
          	inner join producto pd on pd.producto_id = tr.id_producto
          	inner join condiciones_pago cp on cp.id_condiciones = v.condicion_pago
          	inner join unidades u on u.id_unidad = tr.unidad_medida
          	inner join impuestos i on i.id_impuesto = pd.producto_impuesto
          	inner join unidades_has_producto up on up.producto_id = pd.producto_id and up.id_unidad=tr.unidad_medida
          	left join credito cre on cre.id_venta=v.venta_id
          where v.venta_id = 387 order by 1
