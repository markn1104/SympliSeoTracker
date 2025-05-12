import axios, { AxiosInstance,AxiosResponse } from "axios";
import qs from "qs";

const _configRequest = async (request: any) => {
  if (!request.params) {
    request.params = {};
  }
  request.baseURL = "http://localhost:5184/api/Search";

  request.paramsSerializer = (params: any) => qs.stringify(params);
  return request;
};

const _configResponse = async (response: AxiosResponse<any>) => {
  return response.data;
};

const _configError = async (error: any) => {
  console.log(error);
  const messageError = error.response.data?.message || error.message;
  if (axios.isCancel(error)) {
    return new Promise((r) => {
      console.log("Cancel:", r);
    });
  }
  if (
    error.request.responseType === "blob" &&
    error.response.data instanceof Blob &&
    error.response.data.type &&
    error.response.data.type.toLowerCase().indexOf("json") !== -1
  ) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        error.response.data = JSON.parse(reader.result as string);
        resolve(
          Promise.reject({
            ...error,
            message: messageError,
          })
        );
      };

      reader.onerror = () => {
        reject({
          ...error,
          message: messageError,
        });
      };
      reader.readAsText(error?.response?.data);
    });
  }

  return Promise.reject({
    ...error,
    message: messageError,
  });
};
const axiosInstance: AxiosInstance = axios.create({
  baseURL: process.env.NEXT_PUBLIC_BACKEND,
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*",
  },
});
const APIService = axiosInstance as {
  get<T = any>(url: string, config?: any): Promise<T>;
};

axiosInstance.interceptors.request.use(_configRequest, _configError);
axiosInstance.interceptors.response.use(_configResponse, _configError);

export default APIService;
