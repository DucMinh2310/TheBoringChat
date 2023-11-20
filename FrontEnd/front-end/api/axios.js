import Axios from 'axios';
import Cookies from 'js-cookie';

const api = Axios.create({
  timeout: 3 * 60 * 1000,
  baseURL: process.env.NEXT_PUBLIC_BASE_URL,
});

api.interceptors.request.use(
  (config) => {
    const token = Cookies.get('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if ( error.response.status === 503 ) {
      return redirectMaintenancePage();
    } 
    deleteStatusMaintenance();
    if (error.response.status === 404) {
      return getNotFound();
    }
  
    if (error.response.status === 500) {
      return getInternalServerError();
    }
  
    if (error.response.status === 403) {
      return logout();
    }

    if (error.response.status === 401) {
      return logout();
    }

      return Promise.reject(error);
    }
);

const logout = async () => {
  // eslint-disable-next-line import/no-cycle
  const store = await import('../redux/store');
  Cookies.remove('accessToken');
  store.default.dispatch({
    type: 'auth/authUserByAccessToken',
    payload: false,
  });

  store.default.dispatch({
    type: 'setIsLogoutEvent',
    payload: true,
  })

  window.location.href = `${process.env.NEXT_PUBLIC_BASE_URL}/login`
  
};

const deleteStatusMaintenance = async () => {
  // eslint-disable-next-line import/no-cycle
  const store = await import('../redux/store');
  const isMaintenance = store.default.getState().auth.maintenance
  if ( isMaintenance ) {
    store.default.dispatch({
      type: 'auth/checkErrorStatus',
      payload: false,
    });
  }
};

const redirectMaintenancePage = async () => {
  // eslint-disable-next-line import/no-cycle
  const store = await import('../redux/store');
  store.default.dispatch({ type: 'auth/checkErrorStatus', payload: true });
};

const getNotFound = async () => {
  // eslint-disable-next-line import/no-cycle
  const store = await import('../redux/store');
  store.default.dispatch({ type: 'auth/checkNotFound', payload: true });
};

const getInternalServerError = async () => {
  // eslint-disable-next-line import/no-cycle
  const store = await import('../redux/store');
  store.default.dispatch({
    type: 'auth/checkInternalServerError',
    payload: true,
  });
};

export default api;
