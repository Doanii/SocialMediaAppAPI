function set_nav_url(model_name, model_url) {
    const el = document.getElementById("nav-link-" + model_name);
    if (window.location.pathname === model_url || window.location.pathname === model_url + "/") {
        const inner = document.getElementById("nav-link-inner-" + model_name);
        el.innerHTML = inner.innerHTML;

        el.classList.add("text-amber-500");
        el.classList.add("font-medium");
    } else {
        el.classList.add("font-thin");
    }
}