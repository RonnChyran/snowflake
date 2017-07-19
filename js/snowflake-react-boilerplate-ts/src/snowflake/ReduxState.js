import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import * as platformActions from 'actions/platforms'
import * as gameActions from 'actions/games'
import * as stateActions from 'actions/state'

import getDisplayName from 'recompose/getDisplayName'

const ReduxState = (AppComponent) => {
  function mapStateToProps (state, props) {
    return {
      platforms: state.platforms,
      games: state.games,
      state: state.state
    }
  }

  function mapDispatchToProps (dispatch) {
    return {
      actions: {
        platforms: bindActionCreators(platformActions, dispatch),
        games: bindActionCreators(gameActions, dispatch),
        state: bindActionCreators(stateActions, dispatch)
      },
      dispatch
    }
  }

  return connect(mapStateToProps, mapDispatchToProps, null, { pure: false })(class extends Component {
    static displayName = `State(${getDisplayName(AppComponent)})`
    componentWillMount () {
    }

    render () {
      return (
        <AppComponent {...this.props} />
      )
    }
  })
}

export default ReduxState
