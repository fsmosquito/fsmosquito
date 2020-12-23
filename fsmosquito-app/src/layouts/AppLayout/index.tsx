import React from 'react';

import Link from 'next/link';
import { observer } from 'mobx-react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';

import routesConfig from '@src/routesConfig';

import styles from './AppLayout.module.css';

interface AppLayoutProps {
  children: React.ReactNode;
}

const AppLayout = ({ children }: AppLayoutProps) => {
  return (
    <div className="container-fluid vh-100 d-flex flex-column" style={{ padding: 0 }}>
      <Navbar bg="primary" expand="lg" variant="dark" title="FSMosquito Dashboard">
        <Link href="/" passHref={true}>
          <Navbar.Brand>
            <>
              <img
                style={{ color: 'white' }}
                src="/images/FSMosquito-Logo-White.png"
                height="30"
                className="d-inline-block align-top"
                alt="FSMosquito"
              />
            </>
          </Navbar.Brand>
        </Link>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="mr-auto">
            {routesConfig.map((route, ix) => (
              <React.Fragment key={ix}>
                {route.children && (
                  <NavDropdown
                    title={
                      <>
                        <FontAwesomeIcon className={styles.labelIcon} icon={route.icon} />
                        {route.title}
                      </>
                    }
                    id="basic-nav-dropdown"
                  >
                    {route.children.map((route, ix) => (
                      <Link href={route.url} passHref={true} key={ix}>
                        <NavDropdown.Item>
                          <FontAwesomeIcon className={styles.labelIcon} icon={route.icon} />
                          {route.title}
                        </NavDropdown.Item>
                      </Link>
                    ))}
                  </NavDropdown>
                )}
                {!route.children && (
                  <Link href={route.url} passHref={true}>
                    <Nav.Link>
                      <FontAwesomeIcon className={styles.labelIcon} icon={route.icon} />
                      {route.title}
                    </Nav.Link>
                  </Link>
                )}
              </React.Fragment>
            ))}
          </Nav>
        </Navbar.Collapse>
      </Navbar>
      <Container as="main" className="justify-content-center" fluid style={{ height: '100%', padding: 0 }}>
        {children}
      </Container>
    </div>
  );
};

export default observer(AppLayout);
