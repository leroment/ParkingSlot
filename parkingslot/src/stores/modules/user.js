/* eslint-disable no-unused-vars */
import axios from 'axios';
import store from '../store';

export default {
    state: {
        firstName: "",
        lastName: "",
        username: "",
        email: "",
        phoneNo: "",
        role: "",
        token: localStorage.getItem('access_token') || null,
        isLoggedIn: false,
        userid: ""
    },
    getters: {
        FIRSTNAME: (state) => {
            return state.firstName;
        },
        LASTNAME: (state) => {
            return state.lastName;
        },
        USERNAME: state => {
            return state.username;
        },
        EMAIL: (state) => {
            return state.email;
        },
        PHONENO: (state) => {
            return state.phoneNo;
        },
        ROLE: (state) => {
            return state.role;
        },
        ISLOGGEDIN:(state) => {
            return state.isLoggedIn;
        },
        TOKEN:(state) => {
            return state.token;
        },
        USERID:(state) => {
            return state.userid;
        }
    },
    mutations: {
        SETAUTHSTATUS(state, isLoggedIn) {
            state.isLoggedIn = isLoggedIn;
        },
        SETFIRSTNAME(state, firstName) {
            state.firstName = firstName;
        },
        SETLASTNAME(state, lastName) {
            state.lastName = lastName;
        },
        SETUSERNAME(state, username) {
            state.username = username;
        },
        SETEMAIL(state, email) {
            state.email = email;
        },
        SETPHONENO(state, phoneNo) {
            state.phoneNo = phoneNo;
        },
        SETROLE(state, role) {
            state.role = role;
        },
        SETTOKEN(state, token) {
            state.token = token;
        },
        SETUSERID(state, userid) {
            state.userid = userid;
        }
    },
    actions: {
        LOGIN: ({ commit }, payload) => {
            return new Promise((resolve, reject) => {
                /* POST to the Web API */
                axios.post(`https://parkingslotapi.azurewebsites.net/api/users/authenticate`, payload).then(({ data, status }) => {
                    if (status === 200) {
                        //Add user info to store
                        store.commit('SETUSERNAME', data.username); 
                        store.commit('SETEMAIL', data.email);
                        store.commit('SETFIRSTNAME', data.firstName); 
                        store.commit('SETLASTNAME', data.lastName); 
                        store.commit('SETPHONENO', data.phoneNumber); 
                        store.commit('SETROLE', data.role); 
                        store.commit('SETTOKEN', data.token);
                        store.commit('SETUSERID', data.id);
                        store.commit('SETAUTHSTATUS', true);    
                        resolve(true);
                    }
                }).catch(error => {
                    reject(error);
                })
             
            });
        },
        REGISTER: ({ commit }, payload) => {
            /* POST to the Web API */
            return new Promise((resolve, reject) => {
                axios
                    .post(`https://parkingslotapi.azurewebsites.net/api/users`, payload)
                    .then(({ data, status }) => {
                        if (status === 200) {
                            resolve(true);
                        }
                    })
                    .catch(error => {
                        //console.log(error.response.data);
                        //console.log(error.response.status);
                        reject(error);
                    });
            });
        },
        UPDATE: ({ commit }, payload) => {
            return new Promise((resolve, reject) => {
                /* PUT to the Web API */
                axios.put(`https://parkingslotapi.azurewebsites.net/api/users/${this.$store.getters.USERID}`, payload).then(({ data, status }) => {
                    if (status === 200) {
                        //Update user info to store
                        //console.log(data);
                        // store.commit('SETUSERNAME', data.username); 
                        // store.commit('SETEMAIL', data.email);
                        // store.commit('SETFIRSTNAME', data.firstName); 
                        // store.commit('SETLASTNAME', data.lastName); 
                        // store.commit('SETPHONENO', data.phoneNumber); 
                        // store.commit('SETROLE', data.role); 
                        // store.commit('SETTOKEN', data.token);
                        // store.commit('SETUSERID', data.id);
                        // store.commit('SETAUTHSTATUS', true);    
                        resolve(true);
                    }
                }).catch(error => {
                    reject(error);
                })
             
            });
        },
        CHANGEPASSWORD: ({ commit }, payload) => {
            return new Promise((resolve, reject) => {
                /* PUT to the Web API */
                axios.post(`https://parkingslotapi.azurewebsites.net/api/users/ResetPassword`, payload).then(({ data, status }) => {
                    if (status === 200) {
                        //Update user password
                        resolve(true);
                    }
                }).catch(error => {
                    reject(error);
                })
             
            });
        }
    }
};
