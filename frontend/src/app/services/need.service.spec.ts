import { TestBed } from '@angular/core/testing';

import { NeedService } from './need.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';

describe('NeedService', () => {
  let service: NeedService;
  let httpMock : HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[
        HttpClientTestingModule,
        ],
        providers:[
          NeedService,
        ],
        schemas:[
            CUSTOM_ELEMENTS_SCHEMA,
            NO_ERRORS_SCHEMA
        ]
    });
    service = TestBed.inject(NeedService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
