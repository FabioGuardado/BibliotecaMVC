// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $(document).on('submit', 'form.swal-delete-form', function(e){
        e.preventDefault();
        var form = this;
        Swal.fire({
            title: '¿Eliminar autor?',
            text: 'Esta acción no se puede deshacer.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Si, eliminar',
            cancelButtonText: 'Cancelar'
        })
            .then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
    });

    $(document).on('submit', 'form.swal-save-form', function (e) {
        e.preventDefault();
        var form = this;
        Swal.fire({
            title: '¿Guardar cambios?',
            text: 'Esta acción no se puede deshacer.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Si, guardar',
            cancelButtonText: 'Cancelar'
        })
            .then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
    });
});
