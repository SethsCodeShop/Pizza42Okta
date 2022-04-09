// The Auth0 client, initialized in configureClient()
let auth0 = null;

/**
 * Starts the authentication flow
 */
const login = async (targetUrl) => {
    try {
        const options = {
            redirect_uri: window.location.origin
        };

        if (targetUrl) {
            options.appState = { targetUrl };
        }

        await auth0.loginWithRedirect(options);
    } catch (err) {
        console.log("Log in failed", err);
    }
};

/**
 * Executes the logout flow
 */
const logout = () => {
    try {
        auth0.logout({
            returnTo: window.location.origin
        });
    } catch (err) {
        console.log("Log out failed", err);
    }
};

/**
 * Retrieves the auth configuration from the server
 */
const fetchAuthConfig = () => fetch("./js/auth_config.json");

/**
 * Initializes the Auth0 client
 */
const configureClient = async () => {
    const response = await fetchAuthConfig();
    const config = await response.json();

    auth0 = await createAuth0Client({
        domain: config.domain,
        client_id: config.clientId,
        audience: config.audience,
    });
};

/**
 * Checks to see if the user is authenticated. If so, `fn` is executed. Otherwise, the user
 * is prompted to log in
 * @param {*} fn The function to execute if the user is logged in
 */
const requireAuth = async (fn, targetUrl) => {
    const isAuthenticated = await auth0.isAuthenticated();

    if (isAuthenticated) {
        return fn();
    }

    return login(targetUrl);
};

const PermissionError = "Insufficient permissions. Please talk to your admin if you feel as if this is incorrect.";

const callApi = async () => {
    try {

        // Get the access token from the Auth0 client
        const token = await auth0.getTokenSilently();

        // Make the call to the API, setting the token
        // in the Authorization header
        const response = await fetch("/api/order/claims",
            {
                method: "GET",
                headers:
                {
                    Authorization: `Bearer ${token}`,
                }
            }
        );

        var responseData = "";

        if (response.status == 403) {
            document.getElementById("ERROR").innerHTML = PermissionError;
            return;
        }
        else {
            // Fetch the JSON result
            responseData = await response.json();
        }

        // Display the result in the output element
        const responseElement = document.getElementById("api-call-result");

        responseElement.innerText = JSON.stringify(responseData, {}, 2);

    } catch (e) {
        // Display errors in the console
        console.log(e);
    }
};

const getOrderHistory = async () => {
    try {

        // Get the access token from the Auth0 client
        const token = await auth0.getTokenSilently();

        // Make the call to the API, setting the token
        // in the Authorization header
        const response = await fetch("/api/order/history",
            {
                method: "GET",
                headers:
                {
                    Authorization: `Bearer ${token}`,
                }
            }
        );


        var responseData = "";

        if (response.status == 403) {
            document.getElementById("ERROR").innerHTML = PermissionError;
            return;
        }
        else {
            // Fetch the JSON result
            responseData = await response.json();
        }

        responseData.forEach(function (item) {
            addOrderHistory(item);
        });
    } catch (e) {
        // Display errors in the console
        console.error(e);
    }
};

const getPizzaTypes = async () => {
    try {

        // Get the access token from the Auth0 client
        const token = await auth0.getTokenSilently();

        // Make the call to the API, setting the token
        // in the Authorization header
        const response = await fetch("/api/order/pizzaTypes",
            {
                method: "GET",
                headers:
                {
                    Authorization: `Bearer ${token}`,
                }
            }
        );

        if (response.status == 403) {
            document.getElementById("ERROR").innerHTML = PermissionError;
            return;
        }

        // Fetch the JSON result
        const responseData = await response.json();

        // Display the result in the output element
        const responseElement = document.getElementById("Pizza-Type-Dropdown");

        responseData.forEach(function (item) {
            var option = document.createElement('option');
            option.value = item.id;
            option.innerHTML = item.name;
            responseElement.appendChild(option)
        });

    } catch (e) {
        // Display errors in the console
        console.error(e);
    }
}

const addOrder = async () => {
    try {

        // Get the access token from the Auth0 client
        const token = await auth0.getTokenSilently();

        var request = { PizzaOrderTypeId: document.getElementById('Pizza-Type-Dropdown').value };

        // Make the call to the API, setting the token
        // in the Authorization header
        const response = await fetch("/api/order/add",
            {
                method: "POST",
                headers:
                {
                    Authorization: `Bearer ${token}`,
                    Accept: 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(request)
            }
        );

        if (response.status == 403) {
            document.getElementById("ERROR").innerHTML = PermissionError;
            return;
        }

        // Fetch the JSON result
        const responseData = await response.json();
        addOrderHistory(responseData);
    } catch (e) {
        // Display errors in the console
        if (e.response == "403") {
            console.log('here');
        }
        console.error(e);
    }
};

const addOrderHistory = async (order) => {
    var table = document.getElementById("Order-History-Details");
    var created = getFormattedDate(new Date(order.created));

    var element = document.createElement('tr');
    element.innerHTML = `<th>${order.type.name}</th><th>${created}</th>`;
    table.appendChild(element);
}

function getFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return month + '/' + day + '/' + year;
}

// Will run when page finishes loading
window.onload = async () => {
    await configureClient();

    // If unable to parse the history hash, default to the root URL
    if (!showContentFromUrl(window.location.pathname)) {
        showContentFromUrl("/");
        window.history.replaceState({ url: "/" }, {}, "/");
    }

    const bodyElement = document.getElementsByTagName("body")[0];

    // Listen out for clicks on any hyperlink that navigates to a #/ URL
    bodyElement.addEventListener("click", (e) => {
        if (isRouteLink(e.target)) {
            const url = e.target.getAttribute("href");

            if (showContentFromUrl(url)) {
                e.preventDefault();
                window.history.pushState({ url }, {}, url);
            }
        }
    });

    const isAuthenticated = await auth0.isAuthenticated();

    if (isAuthenticated) {
        window.history.replaceState({}, document.title, window.location.pathname);
        updateUI();
        return;
    }

    const query = window.location.search;
    const shouldParseResult = query.includes("code=") && query.includes("state=");

    if (shouldParseResult) {
        try {
            const result = await auth0.handleRedirectCallback();

            if (result.appState && result.appState.targetUrl) {
                showContentFromUrl(result.appState.targetUrl);
            }
        } catch (err) {
            console.log("Error parsing redirect:", err);
        }

        window.history.replaceState({}, document.title, "/");
    }

    updateUI();
};
