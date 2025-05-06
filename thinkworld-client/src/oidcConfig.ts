import { WebStorageStateStore } from "oidc-client-ts";

const oidcConfig = {
    authority: 'https://dev-99631801-admin.okta.com', // Replace with your Okta domain
    client_id: '0oaogkf8l2iO33Kdk5d7', // Replace with your Okta client ID
    redirect_uri: window.location.origin + '/auth/callback',
    response_type: 'code',
    scope: 'openid profile email',
    post_logout_redirect_uri: window.location.origin,
    usePKCE: false,
    stateStore: new WebStorageStateStore({ store: window.localStorage }),
  };
  

export default oidcConfig;
