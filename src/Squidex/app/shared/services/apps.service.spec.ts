/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { inject, TestBed } from '@angular/core/testing';

import {
    AnalyticsService,
    ApiUrlConfig,
    AppCreatedDto,
    AppDto,
    AppsService,
    DateTime,
    Permission
} from '@app/shared/internal';

describe('AppsService', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule
            ],
            providers: [
                AppsService,
                { provide: ApiUrlConfig, useValue: new ApiUrlConfig('http://service/p/') },
                { provide: AnalyticsService, useValue: new AnalyticsService() }
            ]
        });
    });

    afterEach(inject([HttpTestingController], (httpMock: HttpTestingController) => {
        httpMock.verify();
    }));

    it('should make get request to get apps',
        inject([AppsService, HttpTestingController], (appsService: AppsService, httpMock: HttpTestingController) => {

        let apps: AppDto[];

        appsService.getApps().subscribe(result => {
            apps = result;
        });

        const req = httpMock.expectOne('http://service/p/api/apps');

        expect(req.request.method).toEqual('GET');
        expect(req.request.headers.get('If-Match')).toBeNull();

        req.flush([
            {
                id: '123',
                name: 'name1',
                permissions: ['Owner'],
                created: '2016-01-01',
                lastModified: '2016-02-02',
                planName: 'Free',
                planUpgrade: 'Basic'
            },
            {
                id: '456',
                name: 'name2',
                permissions: ['Owner'],
                created: '2017-01-01',
                lastModified: '2017-02-02',
                planName: 'Basic',
                planUpgrade: 'Enterprise'
            }
        ]);

        expect(apps!).toEqual(
            [
                new AppDto('123', 'name1', [new Permission('Owner')], DateTime.parseISO('2016-01-01'), DateTime.parseISO('2016-02-02'), 'Free', 'Basic'),
                new AppDto('456', 'name2', [new Permission('Owner')], DateTime.parseISO('2017-01-01'), DateTime.parseISO('2017-02-02'), 'Basic', 'Enterprise')
            ]);
    }));

    it('should make post request to create app',
        inject([AppsService, HttpTestingController], (appsService: AppsService, httpMock: HttpTestingController) => {

        const dto = { name: 'new-app' };

        let app: AppCreatedDto;

        appsService.postApp(dto).subscribe(result => {
            app = result;
        });

        const req = httpMock.expectOne('http://service/p/api/apps');

        expect(req.request.method).toEqual('POST');
        expect(req.request.headers.get('If-Match')).toBeNull();

        req.flush({
            id: '123',
            permissions: ['Reader'],
            planName: 'Basic',
            planUpgrade: 'Enterprise'
        });

        expect(app!).toEqual({
            id: '123',
            permissions: ['Reader'],
            planName: 'Basic',
            planUpgrade: 'Enterprise'
        });
    }));

    it('should make delete request to archive app',
    inject([AppsService, HttpTestingController], (appsService: AppsService, httpMock: HttpTestingController) => {

        appsService.deleteApp('my-app').subscribe();

        const req = httpMock.expectOne('http://service/p/api/apps/my-app');

        expect(req.request.method).toEqual('DELETE');
        expect(req.request.headers.get('If-Match')).toBeNull();

        req.flush({});
    }));
});