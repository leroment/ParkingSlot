/* eslint-disable no-unused-vars */
import axios from 'axios';
import store from '../store';

export default {
    state: {
        isLoggedIn: false,
        username: '',
        email: ''

    },
    getters: {
        USERNAME: state => {
            return state.username;
        },
        EMAIL: (state) => {
            return state.email;
        },
        ISLOGGEDIN:(state) => {
            return state.isLoggedIn;
        },
    },
    mutations: {
        SETAUTHSTATUS(state, isLoggedIn) {
            state.isLoggedIn = isLoggedIn;
        },
        SETEMAIL(state, email) {
            state.email = email;
        },
        SETUSERNAME(state, username) {
            state.username = username;
        }
    },
    actions: {
        LOGIN: ({ commit }, { username, password }) => {
            return new Promise((resolve, reject) => {
                /* POST to the Web API */
                /* axios.post(`login`, payload).then(({ data, status }) => {
                    if (status === 200) {
                        resolve(true);
                    }
                }).catch(error => {
                    reject(error);
                })*/

                /* expected data return from axios */
                var username = "John Doe";
                var email = "JohnDoe@gmail.com";
                store.commit('SETAUTHSTATUS', true);
                store.commit('SETUSERNAME',username);
                store.commit('SETEMAIL',email);
                //For testing, resolve(true) for now
                resolve(true);               
            });
        },
        REGISTER: ({ commit }, payload) => {
            /* POST to the Web API */
            return new Promise((resolve, reject) => {
                /* axios
                    .post(`register`, payload)
                    .then(({ data, status }) => {
                        if (status === 201) {
                            resolve(true);
                        }
                    })
                    .catch(error => {
                        reject(error);
                    }); */

                //For testing, resolve(true) for now
                resolve(true);
            });
        },
    }
};