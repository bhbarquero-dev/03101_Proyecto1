// Busqueda instantanea de tickets (HU-07).
// Consulta /Tickets/Buscar mientras se escribe y reemplaza las filas de la tabla sin recargar la pagina.
$(function () {
    var $form = $('#form-busqueda');
    if ($form.length === 0) return;

    var $texto = $('#texto-busqueda');
    var $estado = $('#estado-busqueda');
    var $tabla = $('#tabla-tickets');
    var $filas = $tabla.find('tbody');
    var $total = $('#total-tickets');
    var $mensaje = $('#mensaje-busqueda');
    var urlDetalle = $form.data('url-detalle');
    var conSolicitante = $tabla.data('con-solicitante') === true;
    var columnas = $tabla.find('thead th').length;

    var espera = null;     // temporizador para no consultar en cada tecla
    var solicitud = null;  // solicitud en curso, se cancela si llega otra

    // .text() inserta el contenido como texto, nunca como HTML
    function celda(texto) {
        return $('<td></td>').text(texto);
    }

    function fila(ticket) {
        var $titulo = $('<a></a>').attr('href', urlDetalle + '/' + ticket.id).text(ticket.titulo);
        var $badge = $('<span></span>').attr('class', 'badge ' + ticket.clase).text(ticket.estado);
        var $asignado = ticket.asignado
            ? $('<td></td>').text(ticket.asignado)
            : $('<td><span class="text-muted fst-italic">Sin asignar</span></td>');

        var $tr = $('<tr></tr>').append(celda(ticket.id), $('<td></td>').append($titulo));
        if (conSolicitante) $tr.append(celda(ticket.solicitante));
        return $tr.append(
            celda(ticket.categoria),
            $('<td></td>').append($badge),
            $asignado,
            celda(ticket.creado).addClass('text-nowrap'),
            celda(ticket.actualizado).addClass('text-nowrap'));
    }

    function mostrar(tickets) {
        $filas.empty();
        if (tickets.length === 0) {
            $('<tr></tr>').append(
                $('<td class="text-center text-muted py-3"></td>')
                    .attr('colspan', columnas)
                    .text('No se encontraron tickets con ese criterio.'))
                .appendTo($filas);
        } else {
            $.each(tickets, function (i, ticket) { $filas.append(fila(ticket)); });
        }
        $total.text('Total: ' + tickets.length + ' ticket(s).');
    }

    function buscar() {
        if (solicitud) solicitud.abort();
        $mensaje.removeClass('text-danger').text('Buscando…');

        solicitud = $.ajax({ url: $form.attr('action'), method: 'GET', data: $form.serialize(), dataType: 'json', cache: false })
            .done(function (respuesta) {
                mostrar(respuesta.tickets);
                $mensaje.text('');
            })
            .fail(function (xhr, estado) {
                if (estado === 'abort') return;
                $mensaje.addClass('text-danger').text('No se pudo realizar la búsqueda. Intente de nuevo.');
            })
            .always(function () {
                solicitud = null;
            });
    }

    function programarBusqueda() {
        clearTimeout(espera);
        espera = setTimeout(buscar, 300);
    }

    $texto.on('input', programarBusqueda);
    $estado.on('change', buscar);

    // Enter no envia el formulario: la busqueda ya es instantanea
    $form.on('submit', function (e) {
        e.preventDefault();
        clearTimeout(espera);
        buscar();
    });
});
