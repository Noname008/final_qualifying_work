// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Функция для сохранения состояния боковой панели
function saveSidebarState(collapsed) {
    localStorage.setItem('sidebarCollapsed', collapsed);
}

// Функция для загрузки состояния боковой панели
function loadSidebarState() {
    const savedState = localStorage.getItem('sidebarCollapsed');
    if (savedState === 'true') {
        document.getElementById('sidebar').classList.add('collapsed');
        document.getElementById('mainContent').classList.add('expanded');
        const icon = document.querySelector('#sidebarToggle i');
        icon.classList.remove('bi-chevron-left');
        icon.classList.add('bi-chevron-right');
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    loadSidebarState();

    // Обработчик для кнопки переключения
    document.getElementById('sidebarToggle').addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        const isCollapsed = sidebar.classList.contains('collapsed');
        saveSidebarState(!isCollapsed);
    });
});